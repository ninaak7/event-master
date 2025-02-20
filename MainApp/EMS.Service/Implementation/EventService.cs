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
    public class EventService : IEventService
    {
        private readonly IRepository<Event> _eventRepository;

        public EventService(IRepository<Event> eventRepository)
        {
            _eventRepository = eventRepository;
        }

        public void CreateNewEvent(Event e)
        {
            _eventRepository.Insert(e);
        }

        public void DeleteEvent(Guid id)
        {
            _eventRepository.Delete(_eventRepository.Get(id));
        }

        public List<Event> GetAllEvents()
        {
            return _eventRepository.GetAll().ToList();
        }

        public Event GetDetailsForEvent(Guid? id)
        {
            return _eventRepository.Get(id);
        }

        public void UpdateExistingEvent(Event e)
        {
            _eventRepository.Update(e);
        }
    }
}
