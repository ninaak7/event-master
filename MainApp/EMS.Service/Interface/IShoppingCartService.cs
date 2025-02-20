using EMS.Domain.DTO;
using EMS.Domain.Models;

namespace EMS.Service.Interface
{
    public interface IShoppingCartService
    {
        ShoppingCartDTO GetShoppingCartInfo(string userId);
        bool DeleteTicketFromShoppingCart(string userId, Guid ticketId);
        bool Order(string userId, string stripeSessionId);
        bool ApproveOrder(string userId, string stripeSessionId);
        bool AddToShoppingCartConfirmed(TicketInShoppingCart model, string userId);
    }
}
