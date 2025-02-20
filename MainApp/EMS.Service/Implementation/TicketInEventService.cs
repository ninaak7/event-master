using EMS.Domain.Models;
using EMS.Repository.Interface;
using EMS.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Service.Implementation
{
    public class TicketInEventService : ITicketInEventService
    {
        private readonly IRepository<TicketInEvent> _ticketInEventRepository;

        public TicketInEventService(IRepository<TicketInEvent> ticketInEventRepository)
        {
            _ticketInEventRepository = ticketInEventRepository;
        }

        public void CreateNewTicketInEvent(TicketInEvent t)
        {
            _ticketInEventRepository.Insert(t);
        }

        public void DeleteTicketInEvent(Guid id)
        {
            _ticketInEventRepository.Delete(_ticketInEventRepository.Get(id));  
        }

        public List<TicketInEvent> GetAllTicketsInEvents()
        {
            return _ticketInEventRepository.GetAll().ToList();
        }

        public TicketInEvent GetDetailsForTicketInEvent(Guid? id)
        {
            return _ticketInEventRepository.Get(id);
        }

        public void UpdateExistingTicketInEvent(TicketInEvent t)
        {
            _ticketInEventRepository.Update(t);
        }
    }
}
