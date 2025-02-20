using EMS.Domain.Models;
using EMS.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Service.Interface
{
    public interface IEventService
    {
        List<Event> GetAllEvents();
        Event GetDetailsForEvent(Guid? id);
        void CreateNewEvent(Event e);
        void UpdateExistingEvent(Event e);
        void DeleteEvent(Guid id);
    }
}
