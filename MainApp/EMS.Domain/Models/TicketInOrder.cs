using System.ComponentModel.DataAnnotations;

namespace EMS.Domain.Models
{
    public class TicketInOrder : BaseEntity
    {
        public Guid TicketId { get; set; }
        public TicketInEvent? OrderedTicket { get; set; }
        public Guid OrderId { get; set; }
        public Order? Order { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than zero.")]
        public int Quantity { get; set; }
    }
}
