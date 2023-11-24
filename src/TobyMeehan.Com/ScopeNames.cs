namespace TobyMeehan.Com;

public static class ScopeNames
{
    public static IEnumerable<string> GetAll(bool includeGroup = false)
    {
        if (includeGroup)
        {
            yield return Account.Group;
            yield return Applications.Group;
            yield return Transactions.Group;
        }
        
        yield return Account.Identify;
        yield return Account.Password;
        yield return Account.Update;
        yield return Account.Delete;

        yield return Applications.Create;
        yield return Applications.Read;
        yield return Applications.Update;
        yield return Applications.Delete;

        yield return Transactions.Send;
        yield return Transactions.Transfer;
        yield return Transactions.Read;
    }
    
    public static class Account
    {
        public const string Group = "account";
        public const string Identify = $"{Group}.identify";
        public const string Password = $"{Group}.password";
        public const string Update = $"{Group}.update";
        public const string Delete = $"{Group}.delete";
    }
    
    public static class Applications
    {
        public const string Group = "applications";
        public const string Create = $"{Group}.create";
        public const string Read = $"{Group}.read";
        public const string Update = $"{Group}.update";
        public const string Delete = $"{Group}.delete";
    }
    
    public static class Transactions
    {
        public const string Group = "transactions";
        public const string Send = $"{Group}.send";
        public const string Transfer = $"{Group}.transfer";
        public const string Read = $"{Group}.read";
    }
}