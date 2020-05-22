using System;
using System.Collections.Generic;
using System.Text;

namespace TobyMeehan.Com.Data.Authentication
{
    public class AuthenticationResult<T>
    {
        public AuthenticationResult()
        {
            Success = false;
        }

        public AuthenticationResult(T data)
        {
            Success = true;
            Data = data;
        }

        public AuthenticationResult(bool success, T data)
        {
            Success = true;
            Data = data;
        }

        public bool Success { get; set; }
        public T Data { get; set; }
    }
}
