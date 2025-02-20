using EMS.Domain.Models;
using Microsoft.AspNetCore.Identity;

namespace EMS.Domain.Identity
{
    public class EMSApplicationUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Address { get; set; }
        public ShoppingCart? UserCart { get; set; }
        public Calendar? UserCalendar { get; set; }
    }
}
