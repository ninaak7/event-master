using EMS.Domain.Identity;

namespace EMS.Domain.Models
{
    public class Order : BaseEntity
    {
        public string OwnerId { get; set; }
        public EMSApplicationUser? OrderOwner { get; set; }
        public string StripeSessionId { get; set; }
        public virtual ICollection<TicketInOrder>? TicketsInOrder { get; set; }
    }
}
