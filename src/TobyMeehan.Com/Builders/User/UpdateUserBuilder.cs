using TobyMeehan.Com.Models;
using TobyMeehan.Com.Models.User;

namespace TobyMeehan.Com.Builders.User;

/// <summary>
/// Builder structure used to update a user.
/// </summary>
public struct UpdateUserBuilder : IUpdateUser
{
    public UpdateUserBuilder WithUsername(string value) => this with { Username = value };
    public Optional<string> Username { get; set; }

    public UpdateUserBuilder WithPassword(Password value) => this with { Password = value };
    public Optional<Password> Password { get; set; }
    
    /// <summary>
    /// Sets the <see cref="Description"/> property of the builder.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public UpdateUserBuilder WithDescription(string? value) => this with { Description = value };
    
    /// <summary>
    /// The new description of the user.
    /// </summary>
    public Optional<string?> Description { get; set; }

    /// <summary>
    /// Sets the <see cref="DisplayName"/> property of the builder.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public UpdateUserBuilder WithDisplayName(string value) => this with { DisplayName = value };

    /// <summary>
    /// The new (display) name of the user.
    /// </summary>
    public Optional<string> DisplayName { get; set; }

    /// <summary>
    /// Sets the <see cref="Avatar"/> property of the builder.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public UpdateUserBuilder WithAvatar(IFileUpload value) => this with { Avatar = Optional<IFileUpload>.Of(value) };
    
    /// <summary>
    /// The new avatar of the user.
    /// </summary>
    public Optional<IFileUpload> Avatar { get; set; }
}