using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Domain.Models
{
    public class EventInCalendar:BaseEntity
    {
        public Guid EventId { get; set; }
        public Event? Event { get; set; }
        public Guid CalendarId { get; set; }
        public Calendar? Calendar { get; set; }
        [Required]
        public DateTime AddedDate { get; set; } = DateTime.UtcNow;
    }
}
