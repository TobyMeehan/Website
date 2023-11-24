namespace TobyMeehan.Com.Models.Transaction;

public interface ICreateTransfer
{
    Id<IApplication> Application { get; }
    Id<IUser> Sender { get; }
    Id<IUser> Recipient { get; }
    string? Description { get; }
    double Amount { get; }
}