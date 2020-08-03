using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TobyMeehan.Com.Api.Authorization
{
    public class AuthorizationRequirement : IAuthorizationRequirement
    {
        public AuthorizationRequirement(Operation operation)
        {
            Operation = operation;
        }

        public string FailureMessage { get; set; }
        public Operation Operation { get; set; }
    }
}
