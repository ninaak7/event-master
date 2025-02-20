using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Domain.Models
{
    public class Event : BaseEntity
    {
        [Required]
        public String? Name { get; set; }
        [Required]
        public String? Description { get; set; }
        [Required]
        public String? Type { get; set; }
        [Required]
        public DateTime DateTime { get; set; }
        [Required]
        public String? Location { get; set; }
        public virtual string? CreatedBy { get; set; }
        public string? ImageUrl { get; set; }
        public virtual ICollection<TicketInEvent>? TicketsInEvent { get; set; }
    }
}
