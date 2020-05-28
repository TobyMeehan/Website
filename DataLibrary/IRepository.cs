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

        Task<T> GetSingleByAsync(Expression<Predicate<T>> expression);

        Task<T> GetByIdAsync(string id);

        Task AddAsync(object value);

        Task UpdateByAsync(Expression<Predicate<T>> expression, T value);

        Task RemoveByAsync(Expression<Predicate<T>> expression);
    }
}
