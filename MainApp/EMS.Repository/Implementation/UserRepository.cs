using EMS.Domain.Identity;
using EMS.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace EMS.Repository.Implementation
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext context;
        private DbSet<EMSApplicationUser> entities;
        string errorMessage = string.Empty;

        public UserRepository(ApplicationDbContext context)
        {
            this.context = context;
            entities = context.Set<EMSApplicationUser>();
        }

        public IEnumerable<EMSApplicationUser> GetAll()
        {
            return entities.AsEnumerable();
        }

        public EMSApplicationUser Get(string id)
        {
            var strGuid = id.ToString();
            return entities
                .Where(s => s.Id == strGuid)
                .Include(z => z.UserCart)
                    .ThenInclude(uc => uc.TicketsInShoppingCart)
                    .ThenInclude(tsc => tsc.Ticket).SingleOrDefault();

/*            return entities.Include(z => z.UserCart)
                .Include(z => z.UserCart.TicketsInShoppingCart)
                .Include("UserCart.TicketsInShoppingCart.Ticket")
                .SingleOrDefault(s => s.Id == strGuid);*/
        }

        public void Insert(EMSApplicationUser entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            entities.Add(entity);
            context.SaveChanges();
        }

        public void Update(EMSApplicationUser entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            entities.Update(entity);
            context.SaveChanges();
        }

        public void Delete(EMSApplicationUser entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            entities.Remove(entity);
            context.SaveChanges();
        }
    }
}
