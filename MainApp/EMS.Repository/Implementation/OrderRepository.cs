using EMS.Domain.Models;
using EMS.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Repository.Implementation
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext context;
        private DbSet<Order> entities;

        public OrderRepository(ApplicationDbContext context)
        {
            this.context = context;
            entities = context.Set<Order>();
        }

        public List<Order> GetAllOrders()
        {
            return entities
                .Include(z => z.TicketsInOrder)
                .Include(z => z.OrderOwner)
                .Include("TicketsInOrder.OrderedTicket.Event")
                .ToList();
        }

        public Order GetDetailsForOrder(BaseEntity id)
        {
            return entities
                .Include(z => z.TicketsInOrder)
                .Include(z => z.OrderOwner)
                .Include("TicketsInOrder.OrderedTicket")
                .Include("TicketsInOrder.OrderedTicket.Event")
                .SingleOrDefaultAsync(z => z.Id == id.Id).Result;
        }
    }
}
