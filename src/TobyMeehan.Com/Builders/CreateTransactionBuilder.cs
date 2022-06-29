namespace TobyMeehan.Com.Builders;

public struct CreateTransactionBuilder
{
    public CreateTransactionBuilder WithUser(Id<IUser> value) => this with { User = value };
    public Id<IUser> User { get; set; }

    public CreateTransactionBuilder WithApplication(Id<IApplication> value) => this with { Application = value };
    public Id<IApplication> Application { get; set; }

    public CreateTransactionBuilder WithDescription(string value) => this with { Description = value };
    public string Description { get; set; }

    public CreateTransactionBuilder WithAmount(double value) => this with { Amount = value };
    public double Amount { get; set; }
}