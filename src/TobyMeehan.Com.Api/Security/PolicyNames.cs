namespace TobyMeehan.Com.Api.Security;

public class PolicyNames
{
    public class Application
    {
        public class Scope
        {
            public const string Create = "scope.application.create";
            public const string Read = "scope.application.read";
            public const string Update = "scope.application.update";
            public const string Delete = "scope.application.delete";
        }
        
        public class Operation
        {
            public const string Create = "operation.application.create";
            public const string Read = "operation.application.read";
            public const string Update = "operation.application.update";
            public const string Delete = "operation.application.delete";
        }
    }
    
    public class User
    {
        public class Scope
        {
            public const string Identify = "scope.user.identify";
            public const string Update = "scope.user.update";
            public const string Password = "scope.user.password";
            public const string Delete = "scope.user.delete";
        }
        
        public class Operation
        {
            public const string Read = "operation.user.read";
            public const string Identify = "operation.user.identify";
            public const string Update = "operation.user.update";
            public const string Password = "operation.user.password";
            public const string Delete = "operation.user.delete";
        }
    }
}