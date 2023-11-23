namespace TobyMeehan.Com.Models.Avatar;

public interface ICreateAvatar
{
    Id<IUser> User { get; }
    IFileUpload File { get; }
}