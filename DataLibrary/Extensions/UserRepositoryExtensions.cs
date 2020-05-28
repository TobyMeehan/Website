using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using TobyMeehan.Com.Data.Models;
using TobyMeehan.Com.Data.Security;

namespace TobyMeehan.Com.Data.Extensions
{
    public static class UserRepositoryExtensions
    {
        public static Task AddAsync(this IRepository<User> users, string username, string password, int balance = 500)
        {
            return users.AddAsync(new
            {
                Username = username,
                HashedPassword = new Password(password),
                Balance = balance
            });
        }

        public static Task UpdatePasswordAsync(this IRepository<User> users, User user, string password)
        {
            user.HashedPassword = new Password(password);

            return users.UpdateByAsync(u => u.Id == user.Id, user);
        }
    }
}
