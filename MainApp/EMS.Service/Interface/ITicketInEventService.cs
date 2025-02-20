using EMS.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Service.Interface
{
    public interface ITicketInEventService
    {
        List<TicketInEvent> GetAllTicketsInEvents();
        TicketInEvent GetDetailsForTicketInEvent(Guid? id);
        void CreateNewTicketInEvent(TicketInEvent t);
        void UpdateExistingTicketInEvent(TicketInEvent t);
        void DeleteTicketInEvent(Guid id);
    }
}
