using System;
using System.Collections.Generic;
using System.Text;

namespace TobyMeehan.Com.Data.Models
{
    public class AuthenticationResult<T>
    {
        public AuthenticationResult()
        {
            Success = false;
        }

        public AuthenticationResult(T result)
        {
            Success = true;
            Result = result;
        }

        public bool Success { get; set; }

        public T Result { get; set; }

    }
}
