using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TobyMeehan.Com.Data.Models;

namespace TobyMeehan.Com.Data.Repositories
{
    public interface IButtonRepository
    {
        Task<IList<ButtonPress>> GetAsync();

        Task<IList<ButtonPress>> GetByUserAsync(string userId);

        Task AddAsync(string userId, TimeSpan buttonTimeSpan);
    }
}
