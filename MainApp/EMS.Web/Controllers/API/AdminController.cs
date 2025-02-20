using EMS.Domain.Identity;
using EMS.Domain.Models;
using EMS.Repository.Interface;
using EMS.Service.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EMS.Web.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IRepository<Domain.Models.Event> _eventService;
        private readonly UserManager<EMSApplicationUser> _userManager;

        public AdminController(IOrderService orderService, IRepository<Domain.Models.Event> eventService, UserManager<EMSApplicationUser> userManager)
        {
            _orderService = orderService;
            _eventService = eventService;
            _userManager = userManager;
        }

        [HttpGet("[action]")]
        public IActionResult GetAllOrders()
        {
            var orders = _orderService.GetAllOrders();

            if (orders == null || !orders.Any())
            {
                return NotFound("No Orders");
            }

            return Ok(_orderService.GetAllOrders());
        }

        [HttpGet("[action]")]
        public IActionResult GetAllEvents()
        {
            var events = _eventService.GetAll();

            if (events == null || !events.Any())
            {
                return NotFound("No Events");
            }

            return Ok(_eventService.GetAll());
        }

        [HttpPost("[action]")]
        public Order GetDetails(BaseEntity id)
        {
            var order = _orderService.GetDetailsForOrder(id);

            return _orderService.GetDetailsForOrder(id);
        }

        [HttpGet("[action]/{id}")]
        public Event GetEventDetails(Guid id)
        {
            return _eventService.Get(id);
        }
    }
}
