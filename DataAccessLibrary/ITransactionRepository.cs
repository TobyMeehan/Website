using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TobyMeehan.Com.Data;

public interface ITransactionRepository
{
    Task<IReadOnlyList<ITransaction>> GetByUserAsync(Id<IUser> userId, int page = 1, int perPage = 15);

    Task<ITransaction> AddAsync(Action<NewTransaction> transaction);
}

public class NewTransaction
{
    public Id<IUser>? UserId { get; set; } = null;
    public Id<IApplication>? AppId { get; set; } = null;
    public string Description { get; set; } = null;
    public int? Amount { get; set; } = null;
}