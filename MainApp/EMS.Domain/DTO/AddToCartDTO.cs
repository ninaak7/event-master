using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Domain.DTO
{
    public class AddToCartDTO
    {
        public Guid SelectedTicketId { get; set; }
        public string? SelectedTicketName { get; set; }
        public int Quantity { get; set; }
    }
}
