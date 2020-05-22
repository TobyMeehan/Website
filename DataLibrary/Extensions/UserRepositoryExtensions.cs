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
            string hashed = BCrypt.HashPassword(password, BCrypt.GenerateSalt());

            return users.AddAsync(new
            {
                Username = username,
                HashedPassword = hashed,
                Balance = balance
            });
        }
    }
}
