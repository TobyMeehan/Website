using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TobyMeehan.Com.Data.Collections;
using TobyMeehan.Com.Data.Models;

namespace TobyMeehan.Com.Data.Repositories
{
    public interface ICommentRepository
    {
        Task<Comment> GetByIdAsync(string id);

        Task<IEntityCollection<Comment>> GetByEntityAsync(string entityId);

        Task<Comment> AddAsync(string entityId, string userId, string content);

        Task UpdateAsync(string id, string content);

        Task DeleteAsync(string id);
    }
}
