using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TobyMeehan.Com.Data.Models;
using TobyMeehan.Com.Data.Security;

namespace TobyMeehan.Com.Data.Authentication
{
    public class UserAuthentication : IAuthentication<User>
    {
        private readonly IRepository<User> _users;

        public UserAuthentication(IRepository<User> users)
        {
            _users = users;
        }

        public async Task<AuthenticationResult<User>> AuthenticateAsync(params object[] credentials)
        {
            string username = (string)credentials[0];
            string password = (string)credentials[1];

            IEnumerable<User> users = await _users.GetByAsync(x => x.Username == username);

            return new AuthenticationResult<User>(
                users.Any(x => BCrypt.CheckPassword(password, x.HashedPassword)),
                users.FirstOrDefault());
        }
    }

    public static class UserAuthenticationExtensions
    {
        public static Task<AuthenticationResult<User>> CheckPasswordAsync(this IAuthentication<User> auth, string username, string password)
            => auth.AuthenticateAsync(username, password);
    }
}
