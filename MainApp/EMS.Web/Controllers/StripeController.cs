using EMS.Domain.Identity;
using EMS.Domain.Models;
using EMS.Service.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;

namespace EMS.Web.Controllers
{
    public class StripeController : Controller
    {
        private readonly StripeSettings _stripeSettings;
        private readonly IShoppingCartService shoppingCartService;
        private readonly IMailService _mailService;
        private readonly UserManager<EMSApplicationUser> _userManager;
        private static string _clientUrl = string.Empty;

        public StripeController(IOptions<StripeSettings> stripeSettings, IMailService mailService, IShoppingCartService shoppingCartService, UserManager<EMSApplicationUser> userManager)
        {
            _stripeSettings = stripeSettings.Value;
            _mailService = mailService;
            _userManager = userManager;

            if (string.IsNullOrEmpty(_stripeSettings.SecretKey))
            {
                throw new InvalidOperationException("Stripe Secret Key is not configured.");
            }

            StripeConfiguration.ApiKey = _stripeSettings.SecretKey;
            this.shoppingCartService = shoppingCartService;
        }

        public IActionResult PaymentPage(string email, decimal totalPrice)
        {
            ViewBag.TotalPrice = totalPrice;
            ViewBag.Email = email;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreatePaymentSession(string email, decimal totalPrice)
        {
            if (string.IsNullOrEmpty(StripeConfiguration.ApiKey))
            {
                return BadRequest("Stripe API key is missing.");
            }

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                CustomerEmail = email,
                LineItems = new List<SessionLineItemOptions>
        {
            new()
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    Currency = "usd",
                    UnitAmount = (long)(totalPrice * 100), // Convert dollars to cents
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = "Order Payment"
                    },
                },
                Quantity = 1,
            },
        },
                Mode = "payment",
                SuccessUrl = $"{Request.Scheme}://{Request.Host}/Stripe/PaymentSuccess?sessionId={{CHECKOUT_SESSION_ID}}",
                CancelUrl = $"{Request.Scheme}://{Request.Host}/Stripe/PaymentCancel",
            };

            var service = new SessionService();
            var session = await service.CreateAsync(options);

            var userId = _userManager.GetUserId(User);
            bool createOrder = shoppingCartService.Order(userId, session.Id);

            if (session == null || string.IsNullOrEmpty(session.Url))
            {
                return BadRequest("Failed to create Stripe session.");
            }

            // Redirect user to Stripe Checkout
            return Redirect(session.Url);
        }




        [HttpGet]
        public async Task<IActionResult> PaymentSuccess(string sessionId)
        {
            if (string.IsNullOrEmpty(sessionId))
            {
                return BadRequest("Invalid session ID.");
            }

            var service = new SessionService();
            try
            {
                var session = await service.GetAsync(sessionId); // Get the actual session from Stripe
                if (session.PaymentStatus == "paid")
                {
                    await SendEmailToCurrentUser();
                    var userId = _userManager.GetUserId(User);
                    bool createOrder = shoppingCartService.ApproveOrder(userId, sessionId);
                    return View("PaymentSuccess");
                }
                else
                {
                    TempData["ErrorMessage"] = "Payment was not completed.";
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (StripeException ex)
            {
                return BadRequest($"Stripe error: {ex.Message}");
            }
        }



        public async Task<bool> SendEmailToCurrentUser()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser != null)
            {
                var mailData = new MailData
                {
                    ToEmail = currentUser.Email,
                    ToName = currentUser.UserName,
                    Subject = "Order Confirmation",
                    Body = "<p>Your order has been successful. Than you for using our system!.</p>"
                };

                await _mailService.SendEmailToUser(currentUser.Id, mailData.Subject, mailData.Body);

                return true;  // Return a success view or message.
            }

            return false; // Handle error if the user is not found.
        }

        [HttpGet]
        public IActionResult PaymentCancel()
        {
            TempData["ErrorMessage"] = "Payment canceled. Please try again.";
            return RedirectToAction("Index", "Home");
        }
    }
}
