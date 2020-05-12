using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TobyMeehan.Com.Data.Models;

namespace TobyMeehan.Com.Data
{
    public interface IRepository<T>
    {
        Task<IEnumerable<T>> GetAsync();

        Task<IEnumerable<T>> GetByAsync(Expression<Predicate<T>> expression);

        Task<T> GetByIdAsync(string id);

        Task AddAsync(object value);

        Task UpdateAsync(T value);

        Task RemoveAsync(Expression<Predicate<T>> expression);

        Task RemoveByIdAsync(string id);
    }
}
