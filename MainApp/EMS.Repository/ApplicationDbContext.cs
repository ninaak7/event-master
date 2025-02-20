using EMS.Domain.Identity;
using EMS.Domain.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EMS.Repository
{
    public class ApplicationDbContext : IdentityDbContext<EMSApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Ticket> Tickets { get; set; }
        public virtual DbSet<Event> Events { get; set; }
        public virtual DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<TicketInEvent> TicketsInEvents { get; set; }
        public virtual DbSet<TicketInShoppingCart> TicketInShoppingCarts { get; set; }
        public virtual DbSet<TicketInOrder> TicketInOrders { get; set; }
        public virtual DbSet<EMSApplicationUser> EMSApplicationUsers { get; set; }
        public virtual DbSet<Calendar> Calendars { get; set; }
        public virtual DbSet<EventInCalendar> EventsInCalendars { get; set; }
        public virtual DbSet<StripeSettings> StripeSettings { get; set; }
    }
}
