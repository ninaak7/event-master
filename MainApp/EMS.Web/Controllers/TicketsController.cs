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

namespace EMS.Web.Controllers
{
    public class TicketsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ITicketService _ticketService;
        private readonly IShoppingCartService _shoppingCartService;

        public TicketsController(ApplicationDbContext context, ITicketService ticketService, IShoppingCartService shoppingCartService)
        {
            _context = context;
            _ticketService = ticketService;
            _shoppingCartService = shoppingCartService;
        }

        public IActionResult Index()
        {
            return RedirectToAction("Index", "Events");
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Type,Price,Id")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                ticket.Id = Guid.NewGuid();
                _ticketService.CreateNewTicket(ticket);

                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction("Index", "Events");
        }

        private bool TicketExists(Guid id)
        {
            return _ticketService.GetDetailsForTicket(id) != null;
        }
    }
}
