using EMS.Domain.DTO;
using EMS.Domain.Models;
using EMS.Repository.Interface;
using EMS.Service.Interface;
using Microsoft.IdentityModel.Tokens;

namespace EMS.Service.Implementation
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly IRepository<ShoppingCart> _shoppingCartRepository;
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<TicketInOrder> _ticketsInOrderRepository;
        private readonly IRepository<TicketInShoppingCart> _ticketsInShoppingCartRepository;
        private readonly IUserRepository _userRepository;
        private readonly IRepository<Ticket> _ticketRepository;
        private readonly IRepository<TicketInEvent> _ticketInEventRepository;

        public ShoppingCartService(IRepository<ShoppingCart> shoppingCartRepository, IRepository<Order> orderRepository, IRepository<TicketInOrder> ticketsInOrderRepository, IRepository<TicketInShoppingCart> ticketsInShoppingCartRepository, IUserRepository userRepository, IRepository<Ticket> ticketRepository, IRepository<TicketInEvent> ticketInEventRepository)
        {
            _shoppingCartRepository = shoppingCartRepository;
            _orderRepository = orderRepository;
            _ticketsInOrderRepository = ticketsInOrderRepository;
            _ticketsInShoppingCartRepository = ticketsInShoppingCartRepository;
            _userRepository = userRepository;
            _ticketRepository = ticketRepository;
            _ticketInEventRepository = ticketInEventRepository;
        }

        public bool AddToShoppingCartConfirmed(TicketInShoppingCart model, string userId)
        {
            var user = _userRepository.Get(userId);
            var shoppingCart = user.UserCart;

            TicketInShoppingCart newTicket = new TicketInShoppingCart
            {
                Id = Guid.NewGuid(),
                Ticket = model.Ticket,
                TicketId = model.TicketId,
                ShoppingCart = shoppingCart,
                ShoppingCartId = shoppingCart.Id,
                Quantity = model.Quantity
            };

            _ticketsInShoppingCartRepository.Insert(newTicket);

            return true;
        }

        public bool DeleteTicketFromShoppingCart(string userId, Guid ticketId)
        {
            if (!string.IsNullOrEmpty(userId) && ticketId != null)
            {
                var user = _userRepository.Get(userId);
                var shoppingCart = user.UserCart;

                var itemToDelete = shoppingCart.TicketsInShoppingCart.Where(z => z.TicketId.Equals(ticketId)).FirstOrDefault();

                shoppingCart.TicketsInShoppingCart.Remove(itemToDelete);

                _shoppingCartRepository.Update(shoppingCart);

                return true;
            }

            return false;
        }

        public ShoppingCartDTO GetShoppingCartInfo(string userId)
        {
            var user = _userRepository.Get(userId);
            var shoppingCart = user.UserCart;

            var total = 0.0;
            int removedTickets = 0;
            var validTickets = new List<TicketInShoppingCart>();

            foreach (var item in shoppingCart.TicketsInShoppingCart.ToList())
            {
                var ticketEvent = _ticketInEventRepository.Get(item.TicketId);

                if (ticketEvent != null && ticketEvent.Quantity >= item.Quantity)
                {
                    validTickets.Add(item);
                }
                else
                {
                    _ticketsInShoppingCartRepository.Delete(item);
                    removedTickets++;
                }
            }

            shoppingCart.TicketsInShoppingCart = validTickets;
            _shoppingCartRepository.Update(shoppingCart);

            foreach (var item in shoppingCart.TicketsInShoppingCart)
            {
                var quantity = item.Quantity;
                var ticket = _ticketRepository.Get(item.Ticket.TicketId);
                var price = ticket.Price;
                total += quantity * price;
            }

            ShoppingCartDTO selectedShoppingCart = new ShoppingCartDTO
            {
                AllTickets = (shoppingCart.TicketsInShoppingCart ?? new List<TicketInShoppingCart>()).ToList<TicketInShoppingCart>(),
                TotalPrice = total,
                RemovedTicketsCount = removedTickets,
            };

            return selectedShoppingCart;
        }

        public bool Order(string userId, string stripeSessionId)
        {
            var user = _userRepository.Get(userId);
            var shoppingCart = user.UserCart;

            if (user == null || shoppingCart == null || stripeSessionId.IsNullOrEmpty() || !user.UserCart.TicketsInShoppingCart.Any())
            {
                return false;
            }

            var newOrder = new Order
            {
                Id = Guid.NewGuid(),
                OwnerId = userId,
                OrderOwner = user,
                StripeSessionId = stripeSessionId,
                TicketsInOrder = new List<TicketInOrder>()
            };

            foreach (var ticketInCart in shoppingCart.TicketsInShoppingCart)
            {
                var newTicketInOrder = new TicketInOrder
                {
                    Id = Guid.NewGuid(),
                    TicketId = ticketInCart.TicketId,
                    Quantity = ticketInCart.Quantity,
                    OrderId = newOrder.Id
                };

                newOrder.TicketsInOrder.Add(newTicketInOrder);

                var eventTickets = _ticketInEventRepository.Get(ticketInCart.TicketId);
                //eventTickets.Quantity -= ticketInCart.Quantity;
                _ticketInEventRepository.Update(eventTickets);
            }

            _orderRepository.Insert(newOrder);
            //shoppingCart.TicketsInShoppingCart = new List<TicketInShoppingCart>();
            //_shoppingCartRepository.Update(shoppingCart);

            return true;
        }

        public bool ApproveOrder(string userId, string stripeSessionId)
        {
            var user = _userRepository.Get(userId.ToString());
            var shoppingCart = user.UserCart;

            if (user == null || shoppingCart == null || stripeSessionId == null || !user.UserCart.TicketsInShoppingCart.Any())
            {
                return false;
            }

            var order = _orderRepository.GetAll().Where(o => (o.OwnerId == userId && o.StripeSessionId == stripeSessionId)).FirstOrDefault();
            var tickets = _ticketsInOrderRepository.GetAll().Where(t => t.OrderId == order.Id);
            if (order == null || tickets == null)
            {
                return false;
            }

            foreach (var ticket in tickets)
            {
                var eventTickets = _ticketInEventRepository.Get(ticket.TicketId);
                eventTickets.Quantity -= ticket.Quantity;
                _ticketInEventRepository.Update(eventTickets);
            }

            shoppingCart.TicketsInShoppingCart = new List<TicketInShoppingCart>();
            _shoppingCartRepository.Update(shoppingCart);

            return true;
        }
    }
}
