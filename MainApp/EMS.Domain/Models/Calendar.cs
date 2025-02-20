namespace EMS.Domain.Models
{
    public class Calendar : BaseEntity
    {
        public string OwnerId { get; set; }
        public virtual ICollection<EventInCalendar>? EventsInCalendar { get; set; }
    }
}
