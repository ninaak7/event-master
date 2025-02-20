using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EMS.Domain.Models;
using EMS.Repository;
using EMS.Service.Interface;
using EMS.Service.Implementation;
using System.Security.Claims;
using System.Net.Sockets;

namespace EMS.Web.Controllers
{
    public class TicketInEventsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ITicketService _ticketService;
        private readonly IEventService _eventService;
        private readonly ITicketInEventService _ticketInEventService;
        private readonly IShoppingCartService _shoppingCartService;

        public TicketInEventsController(ApplicationDbContext context, ITicketService ticketService, IEventService eventService, ITicketInEventService ticketInEventService, IShoppingCartService shoppingCartService)
        {
            _context = context;
            _ticketService = ticketService;
            _eventService = eventService;
            _ticketInEventService = ticketInEventService;
            _shoppingCartService = shoppingCartService;
        }

        public IActionResult Index()
        {
            return RedirectToAction("Index", "Events");
        }

        public IActionResult Create(Guid? id)
        {
            ViewData["Tickets"] = _ticketService.GetAllTickets().ToList();
            var e = _eventService.GetDetailsForEvent(id);

            var newTicket = new TicketInEvent
            {
                EventId = e.Id,
            };

            return View(newTicket);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("EventId,TicketId,Quantity,Id")] TicketInEvent ticketInEvent)
        {
            if (ModelState.IsValid)
            {
                ticketInEvent.Id = Guid.NewGuid();
                _ticketInEventService.CreateNewTicketInEvent(ticketInEvent);

                return RedirectToAction("Details", "Events", new { id = ticketInEvent.EventId });
            }

            ViewData["EventId"] = new SelectList(_context.Events, "Id", "Name", ticketInEvent.EventId);
            ViewData["TicketId"] = new SelectList(_context.Tickets, "Id", "Type", ticketInEvent.TicketId);

            return View(ticketInEvent);
        }

        public IActionResult AddToCart(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticketInEvent = _ticketInEventService.GetDetailsForTicketInEvent(id);

            if (ticketInEvent.Quantity <= 0)
            {
                TempData["ErrorMessage"] = "This ticket is sold out.";

                return RedirectToAction("Index", "Events");
            }

            TicketInShoppingCart ticketInCart = new TicketInShoppingCart
            {
                TicketId = ticketInEvent.Id,
                Quantity = 1,
            };

            return View(ticketInCart);
        }

        [HttpPost]
        public IActionResult AddToCartConfirmed(TicketInShoppingCart ticketInCart)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";

            var ticketEvent = _ticketInEventService.GetDetailsForTicketInEvent(ticketInCart.TicketId);

            if (ticketEvent == null)
            {
                TempData["ErrorMessage"] = "This ticket no longer exists.";
                return RedirectToAction("Index", "Events");
            }

            if (ticketInCart.Quantity <= 0)
            {
                ModelState.AddModelError("Quantity", "Quantity must be greater than zero.");
            }

            if (ticketInCart.Quantity > ticketEvent.Quantity)
            {
                ModelState.AddModelError("Quantity", "Not enough tickets available.");
            }

            if (!ModelState.IsValid)
            {
                return View("AddToCart", ticketInCart);
            }

            _shoppingCartService.AddToShoppingCartConfirmed(ticketInCart, userId);

            return RedirectToAction("Index", "ShoppingCarts");
        }

        public IActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticketInEvent = _ticketInEventService.GetDetailsForTicketInEvent(id);

            if (ticketInEvent == null)
            {
                return NotFound();
            }

            ViewData["EventId"] = new SelectList(_context.Events, "Id", "Name", ticketInEvent.EventId);
            ViewData["TicketId"] = new SelectList(_context.Tickets, "Id", "Type", ticketInEvent.TicketId);

            return View(ticketInEvent);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Guid id, [Bind("EventId,TicketId,Quantity,Id")] TicketInEvent ticketInEvent)
        {
            if (id != ticketInEvent.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _ticketInEventService.UpdateExistingTicketInEvent(ticketInEvent);
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }

                return RedirectToAction("Details", "Events", new { id = ticketInEvent.EventId });
            }

            ViewData["EventId"] = new SelectList(_context.Events, "Id", "Name", ticketInEvent.EventId);
            ViewData["TicketId"] = new SelectList(_context.Tickets, "Id", "Type", ticketInEvent.TicketId);

            return View(ticketInEvent);
        }

        public IActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticketInEvent = _ticketInEventService.GetDetailsForTicketInEvent(id);

            if (ticketInEvent == null)
            {
                return NotFound();
            }

            return View(ticketInEvent);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(Guid id)
        {
            var ticketInEvent = _ticketInEventService.GetDetailsForTicketInEvent(id);

            if (ticketInEvent != null)
            {
                _ticketInEventService.DeleteTicketInEvent(id);
            }

            return RedirectToAction("Details", "Events", new { id = ticketInEvent.EventId });
        }

        private bool TicketInEventExists(Guid id)
        {
            return _ticketInEventService.GetDetailsForTicketInEvent(id) != null;
        }
    }
}
