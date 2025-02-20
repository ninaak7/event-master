using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EMS.Domain.Models;
using EMS.Repository;
using EMS.Domain.Identity;
using Microsoft.AspNetCore.Identity;
using EMS.Service.Interface;
using EMS.Service.Implementation;

namespace EMS.Web.Controllers
{
    public class EventsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<EMSApplicationUser> _userManager;
        private readonly IEventService _eventService;
        private readonly IOrderService _orderService;
        private readonly ITicketInOrderService _ticketInOrderService;

        public EventsController(ApplicationDbContext context, UserManager<EMSApplicationUser> userManager, IEventService eventService, IOrderService orderService, ITicketInOrderService ticketInOrderService)
        {
            _context = context;
            _userManager = userManager;
            _eventService = eventService;
            _orderService = orderService;
            _ticketInOrderService = ticketInOrderService;
        }

        public IActionResult Index()
        {
            return View(_eventService.GetAllEvents());
        }

        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = _eventService.GetDetailsForEvent(id);

            if (@event == null)
            {
                return NotFound();
            }

            var ticketsInEvent = _context.TicketsInEvents
                    .Include(t => t.Ticket)
                    .Include(e => e.Event)
                    .Where(t => t.EventId == id)
                    .ToList();

            ViewData["EventTickets"] = ticketsInEvent;

            var orderedTickets = _ticketInOrderService.GetAllTicketsInOrders(id);
            List<EMSApplicationUser> buyers = new List<EMSApplicationUser>();

            foreach(var order in orderedTickets)
            {
                var buyer = await _userManager.FindByIdAsync(order.Order.OwnerId.ToString());

                if(!buyers.Contains(buyer))
                {
                    buyers.Add(buyer);
                }
            }

            ViewData["Buyers"] = buyers;

            return View(@event);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Name,Description,Type,DateTime,Location,CreatedBy,ImageUrl,Id")] Event @event)
        {
            if (ModelState.IsValid)
            {
                var userId = _userManager.GetUserId(User);

                if (userId == null)
                {
                    return Unauthorized();
                }

                @event.CreatedBy = userId;
                @event.Id = Guid.NewGuid();
                _eventService.CreateNewEvent(@event);

                return RedirectToAction("Details", "Events", new { id = @event.Id });
            }

            return View(@event);
        }

        public IActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = _eventService.GetDetailsForEvent(id);

            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Guid id, [Bind("Name,Description,Type,DateTime,Location,CreatedBy,ImageUrl,Id")] Event @event)
        {
            if (id != @event.Id)
            {
                return NotFound();
            }

            bool hasOrderedTickets = _ticketInOrderService.HasOrderedTicketsForEvent(id);

            if (hasOrderedTickets)
            {
                TempData["ErrorMessage"] = "Cannot edit the event because tickets have already been ordered.";

                return RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _eventService.UpdateExistingEvent(@event);
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }

                return RedirectToAction("Details", "Events", new { id = @event.Id });
            }

            return View(@event);
        }

        public IActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = _eventService.GetDetailsForEvent(id);

            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(Guid id)
        {
            bool hasOrderedTickets = _ticketInOrderService.HasOrderedTicketsForEvent(id);

            if (hasOrderedTickets)
            {
                TempData["ErrorMessage"] = "Cannot delete the event because tickets have already been ordered.";

                return RedirectToAction(nameof(Index));
            }

            var @event = _eventService.GetDetailsForEvent(id);

            if (@event != null)
            {
                _eventService.DeleteEvent(id);
            }

            return RedirectToAction(nameof(Index));
        }

        private bool EventExists(Guid id)
        {
            return _eventService.GetDetailsForEvent(id)!=null;
        }
    }
}
