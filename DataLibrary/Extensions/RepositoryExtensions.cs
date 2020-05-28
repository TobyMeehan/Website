using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace TobyMeehan.Com.Data.Extensions
{
    public static class RepositoryExtensions
    {
        public static async Task<bool> AnyAsync<T>(this IRepository<T> repository)
        {
            return (await repository.GetAsync()).Any();
        }

        public static async Task<bool> AnyAsync<T>(this IRepository<T> repository, Expression<Predicate<T>> expression)
        {
            return (await repository.GetByAsync(expression)).Any();
        }
    }
}
