using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Domain.DTO
{
    public class CalendarEventDTO
    {
        public Guid EventId { get; set; }
        public string Name { get; set; }
        public DateTime AddedDate { get; set; }
        public string Location { get; set; }
        public string Status { get; set; }
    }
}
