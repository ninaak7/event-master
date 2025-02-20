using EMS.Domain.DTO;
using EMS.Domain.Models;
using EMS.Repository.Interface;
using EMS.Service.Interface;

namespace EMS.Service.Implementation
{
    public class CalendarService : ICalendarService
    {
        private readonly IRepository<Calendar> _calendarRepository;
        private readonly IRepository<Event> _eventRepository;
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<TicketInShoppingCart> _ticketInShoppingCartRepository;
        private readonly IRepository<TicketInOrder> _ticketInOrderRepository;
        private readonly IUserRepository _userRepository;
        private readonly IRepository<ShoppingCart> _shoppingCartRepository;
        private readonly IRepository<TicketInEvent> _ticketInEventRepository;

        public CalendarService(IRepository<Calendar> calendarRepository, IRepository<Event> eventRepository, IRepository<Order> orderRepository, IRepository<TicketInShoppingCart> ticketInShoppingCartRepository, IRepository<TicketInOrder> ticketInOrderRepository, IUserRepository userRepository, IRepository<ShoppingCart> shoppingCartRepository, IRepository<TicketInEvent> ticketInEventRepository)
        {
            _calendarRepository = calendarRepository;
            _eventRepository = eventRepository;
            _orderRepository = orderRepository;
            _ticketInShoppingCartRepository = ticketInShoppingCartRepository;
            _ticketInOrderRepository = ticketInOrderRepository;
            _userRepository = userRepository;
            _shoppingCartRepository = shoppingCartRepository;
            _ticketInEventRepository = ticketInEventRepository;
        }

        public List<CalendarEventDTO> GetUserCalendarEvents(string userId)
        {
            var allEvents = _eventRepository.GetAll().ToList();

            var userCart = _shoppingCartRepository.GetAll().Where(c => c.OwnerId == userId).First();
            var eventsInCart = _ticketInShoppingCartRepository.GetAll().Where(t => t.ShoppingCartId == userCart.Id).ToList();

            foreach (var e in eventsInCart)
            {
                e.Ticket = _ticketInEventRepository.Get(e.TicketId);
            }

            var userOrders = _orderRepository.GetAll().Where(o => o.OwnerId == userId).ToList();
            var eventsInOrder = new List<TicketInOrder>();

            foreach (var userOrder in userOrders)
            {
                foreach (var ticketsInOrder in _ticketInOrderRepository.GetAll().ToList())
                {
                    if (ticketsInOrder.OrderId == userOrder.Id)
                    {
                        eventsInOrder.Add(ticketsInOrder);
                    }
                }
            }

            foreach (var e in eventsInOrder)
            {
                e.OrderedTicket = _ticketInEventRepository.Get(e.TicketId);
            }

            List<CalendarEventDTO> calendarEvents = new List<CalendarEventDTO>();

            foreach (var e in allEvents)
            {
                var status = " ";
                if (eventsInOrder.Any(t => t.OrderedTicket.EventId == e.Id))
                    status = "Attending ✅";
                else if (eventsInCart.Any(t => t.Ticket.EventId == e.Id))
                    status = "In Cart 🛒";
                else
                    status = "Saved 📅";

                calendarEvents.Add(new CalendarEventDTO
                {
                    EventId = e.Id,
                    Name = e.Name,
                    AddedDate = e.DateTime,
                    Location = e.Location,
                    Status = status
                });
            }

            return calendarEvents;
        }
    }
}
