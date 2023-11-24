using TobyMeehan.Com.Models.Transaction;

namespace TobyMeehan.Com.Builders.Transaction;

public struct CreateTransferBuilder : ICreateTransfer
{
    public CreateTransferBuilder WithApplication(Id<IApplication> value) => this with { Application = value };
    public Id<IApplication> Application { get; set; }

    public CreateTransferBuilder WithRecipient(Id<IUser> value) => this with { Recipient = value };
    public Id<IUser> Recipient { get; set; }

    public CreateTransferBuilder WithSender(Id<IUser> value) => this with { Sender = value };
    public Id<IUser> Sender { get; set; }

    public CreateTransferBuilder WithDescription(string? value) => this with { Description = value };
    public string? Description { get; set; }

    public CreateTransferBuilder WithAmount(double value) => this with { Amount = value };
    public double Amount { get; set; }
    
}