namespace TobyMeehan.Com;

public class Scope
{
    public const string Identify = "identify";
    public const string Transactions = "transactions";
    public const string Downloads = "downloads";

    public static IEnumerable<string> All => new[] { Identify, Transactions, Downloads };
}