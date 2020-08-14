using Microsoft.AspNetCore.Authorization.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace TobyMeehan.Com.AspNetCore.Authorization
{
    public class CreateOperationAuthorizationRequirement : OperationAuthorizationRequirement { }

    public class ReadOperationAuthorizationRequirement : OperationAuthorizationRequirement { }

    public class UpdateOperationAuthorizationRequirement : OperationAuthorizationRequirement { }

    public class DeleteOperationAuthorizationRequirement : OperationAuthorizationRequirement { }

    public static class OperationAuthorizationRequirementExtensions
    {
        public static bool IsCrudOperationRequirement(this OperationAuthorizationRequirement requirement)
        {
            return
                requirement is CreateOperationAuthorizationRequirement ||
                requirement is ReadOperationAuthorizationRequirement ||
                requirement is UpdateOperationAuthorizationRequirement ||
                requirement is DeleteOperationAuthorizationRequirement;
        }
    }
}
