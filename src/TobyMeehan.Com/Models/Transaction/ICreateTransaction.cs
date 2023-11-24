namespace TobyMeehan.Com.Models.Transaction;

public interface ICreateTransaction
{
    Id<IApplication> Application { get; }
    Id<IUser> User { get; }
    string? Description { get; }
    double Amount { get; }
}