using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TobyMeehan.Com.Data.Models;

namespace TobyMeehan.Com.Data.Repositories
{
    public interface ICommentRepository
    {
        Task<Comment> GetByIdAsync(string id);

        Task<IList<Comment>> GetByEntityAsync(string entityId);

        Task AddAsync(string entityId, string userId, string content);
    }
}
