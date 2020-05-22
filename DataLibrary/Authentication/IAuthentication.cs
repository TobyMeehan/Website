using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TobyMeehan.Com.Data.Authentication
{
    public interface IAuthentication<T>
    {
        Task<AuthenticationResult<T>> AuthenticateAsync(params object[] credentials);
    }
}
