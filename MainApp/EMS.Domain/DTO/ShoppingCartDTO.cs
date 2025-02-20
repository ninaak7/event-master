using EMS.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Domain.DTO
{
    public class ShoppingCartDTO
    {
        public List<TicketInShoppingCart>? AllTickets { get; set; }
        public double TotalPrice { get; set; }
        public int RemovedTicketsCount { get; set; } = 0;
    }
}
