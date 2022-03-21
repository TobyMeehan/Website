using System.Collections.Generic;
using System.Threading.Tasks;

namespace TobyMeehan.Com.Data;

public interface IRoleRepository
{
    Task<IReadOnlyList<IRole>> GetAsync();

    Task<IRole> GetByIdAsync(string id);

    Task<IRole> GetByNameAsync(string name);

    Task<IRole> AddAsync(string name);

    Task DeleteAsync(Id<IRole> id);
}