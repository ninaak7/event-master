using EMS.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Service.Interface
{
    public interface ITicketInOrderService
    {
        bool HasOrderedTicketsForEvent(Guid? eventId);
        List<TicketInOrder> GetAllTicketsInOrders(Guid? eventId);
    }
}
