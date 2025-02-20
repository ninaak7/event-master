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
using EMS.Domain.Identity;
using Microsoft.AspNetCore.Identity;

namespace EMS.Web.Controllers
{
    public class CalendarsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ICalendarService _calendarService;
        private readonly UserManager<EMSApplicationUser> _userManager;

        public CalendarsController(ApplicationDbContext context, ICalendarService calendarService, UserManager<EMSApplicationUser> userManager)
        {
            _context = context;
            _calendarService = calendarService;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var userId = _userManager.GetUserId(User);
            var events = _calendarService.GetUserCalendarEvents(userId);

            return View(events);
        }
    }
}
