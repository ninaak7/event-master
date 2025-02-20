using EMS.Domain.Models;
using EMS.Repository;
using EMS.Repository.Interface;
using EMS.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Service.Implementation
{
    public class TicketInOrderService:ITicketInOrderService
    {
        private readonly ApplicationDbContext _context;
        private readonly IRepository<TicketInOrder> _ticketInOrderRepository;
        private readonly IRepository<Event> _eventRepository;
        private readonly IRepository<TicketInEvent> _ticketInEventRepository;
        private readonly IRepository<Order> _orderRepository;

        public TicketInOrderService(ApplicationDbContext context, IRepository<TicketInOrder> ticketInOrderRepository, IRepository<Event> eventRepository, IRepository<TicketInEvent> ticketInEventRepository, IRepository<Order> orderRepository)
        {
            _context = context;
            _ticketInOrderRepository = ticketInOrderRepository;
            _eventRepository = eventRepository;
            _ticketInEventRepository = ticketInEventRepository;
            _orderRepository = orderRepository;
        }

        public List<TicketInOrder> GetAllTicketsInOrders(Guid? eventId)
        {
            var ticketsInOrders = _ticketInOrderRepository.GetAll();

            foreach (var item in ticketsInOrders)
            {
                item.OrderedTicket = _ticketInEventRepository.Get(item.TicketId);
                item.Order = _orderRepository.Get(item.OrderId);
            }

            return ticketsInOrders.Where(z => z.OrderedTicket.EventId == eventId).ToList();
        }

        public bool HasOrderedTicketsForEvent(Guid? eventId)
        {
            var orderedTickets = _ticketInOrderRepository.GetAll();
            var selectedEvent = _eventRepository.Get(eventId);

            foreach (var item in orderedTickets)
            {
                var ticketInEvent = _ticketInEventRepository.Get(item.TicketId);
                item.OrderedTicket = ticketInEvent;
            }

            return orderedTickets.Any(e => e.OrderedTicket != null && e.OrderedTicket.EventId == eventId);
        }
    }
}
