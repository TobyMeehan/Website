using System.Collections.Generic;
using System.Threading.Tasks;

namespace TobyMeehan.Com.Data;

public interface IConnectionRepository
{
    Task<IConnection> GetByIdAsync(Id<IConnection> id);
    
    Task<IReadOnlyList<IConnection>> GetByUserAsync(Id<IUser> userId);

    Task<IReadOnlyList<IConnection>> GetByApplicationAsync(Id<IApplication> appId);

    Task<IConnection> GetOrCreateAsync(Id<IUser> userId, Id<IApplication> appId);

    Task DeleteByApplicationAsync(Id<IApplication> appId);
    
    Task DeleteAsync(Id<IConnection> id);
}