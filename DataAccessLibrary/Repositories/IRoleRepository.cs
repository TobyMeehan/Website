using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TobyMeehan.Com.Data.Collections;
using TobyMeehan.Com.Data.Models;

namespace TobyMeehan.Com.Data.Repositories
{
    public interface IRoleRepository
    {
        Task<IEntityCollection<Role>> GetAsync();

        Task<Role> GetByIdAsync(string id);

        Task<Role> GetByNameAsync(string name);

        Task<Role> AddAsync(string name);

        Task DeleteAsync(string id);
    }
}
