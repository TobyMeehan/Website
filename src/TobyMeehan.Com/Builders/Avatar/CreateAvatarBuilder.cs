using TobyMeehan.Com.Models;
using TobyMeehan.Com.Models.Avatar;

namespace TobyMeehan.Com.Builders.Avatar;

public struct CreateAvatarBuilder : ICreateAvatar
{
    public CreateAvatarBuilder WithUser(Id<IUser> value) => this with { User = value };
    public Id<IUser> User { get; set; }

    public CreateAvatarBuilder WithFile(IFileUpload value) => this with { File = value };
    public IFileUpload File { get; set; }
}