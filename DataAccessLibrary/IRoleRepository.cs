using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TobyMeehan.Com.Data;

public interface IRoleRepository
{
    Task<IReadOnlyList<IRole>> GetAsync();

    Task<IRole> GetByIdAsync(Id<IRole> id);

    Task<IRole> GetByNameAsync(string name);

    Task<IRole> AddAsync(Action<NewRole> role);

    Task DeleteAsync(Id<IRole> id);
}

public class NewRole
{
    public string Name { get; set; }
}