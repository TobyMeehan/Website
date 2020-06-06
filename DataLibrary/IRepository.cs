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
        Task<IList<T>> GetAsync();

        Task<IList<T>> GetByAsync(Expression<Predicate<T>> expression);

        Task<T> GetSingleByAsync(Expression<Predicate<T>> expression);

        Task<IList<T>> GetByAsync<U>(Expression<Func<T, U, bool>> expression);

        Task<T> GetSingleByAsync<U>(Expression<Func<T, U, bool>> expression);

        Task<T> GetByIdAsync(string id);

        Task AddAsync(object value);

        Task UpdateByAsync(Expression<Predicate<T>> expression, object value);

        Task UpdateByIdAsync(string id, object value);

        Task RemoveByAsync(Expression<Predicate<T>> expression);
        Task RemoveByIdAsync(string id);
    }
}
