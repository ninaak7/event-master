using EMS.Domain.Identity;
using EMS.Repository;
using EMS.Repository.Interface;
using EMS.Service.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EMS.Web.Controllers
{
    public class ShoppingCartsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly UserManager<EMSApplicationUser> _userManager;
        private readonly IUserRepository repository;

        public ShoppingCartsController(ApplicationDbContext context, IShoppingCartService shoppingCartService, UserManager<EMSApplicationUser> userManager, IUserRepository userRepo)
        {
            _context = context;
            _shoppingCartService = shoppingCartService;
            _userManager = userManager;
            repository = userRepo;
        }

        public IActionResult Index()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cartInfo = _shoppingCartService.GetShoppingCartInfo(userId);

            if (cartInfo.RemovedTicketsCount > 0)
            {
                ViewData["Message"] = "Some tickets in your cart are no longer available and have been removed.";
            }

            return View(cartInfo);
        }

        private bool ShoppingCartExists(Guid id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            return _shoppingCartService.GetShoppingCartInfo(userId) != null;
        }

        public IActionResult Delete(Guid id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = _shoppingCartService.DeleteTicketFromShoppingCart(userId, id);

            return RedirectToAction("Index", "ShoppingCarts");
        }

        public IActionResult Order()
        {
            var userId = _userManager.GetUserId(User);
            var totalPrice = _shoppingCartService.GetShoppingCartInfo(userId).TotalPrice;
            var email = repository.Get(userId).Address;
            //Console.WriteLine(email);
            return RedirectToAction("PaymentPage", "Stripe", new { email, totalPrice });
        }
    }
}
