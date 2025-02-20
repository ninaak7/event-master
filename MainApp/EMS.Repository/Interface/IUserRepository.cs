using EMS.Domain.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Repository.Interface
{
    public interface IUserRepository
    {
        IEnumerable<EMSApplicationUser> GetAll();
        EMSApplicationUser Get(string id);
        void Insert(EMSApplicationUser entity);
        void Update(EMSApplicationUser entity);
        void Delete(EMSApplicationUser entity);
    }
}
