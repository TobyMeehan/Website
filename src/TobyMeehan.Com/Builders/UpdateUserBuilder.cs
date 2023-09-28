namespace TobyMeehan.Com.Builders;

/// <summary>
/// Builder structure used to update a user.
/// </summary>
public struct UpdateUserBuilder
{
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
    /// Sets the <see cref="Name"/> property of the builder.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public UpdateUserBuilder WithName(string value) => this with { Name = value };
    
    /// <summary>
    /// The new (display) name of the user.
    /// </summary>
    public Optional<string> Name { get; set; }

    /// <summary>
    /// Sets the <see cref="Avatar"/> property of the builder.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public UpdateUserBuilder WithAvatar(FileUploadBuilder value) => this with { Avatar = value };
    
    /// <summary>
    /// The new avatar of the user.
    /// </summary>
    public Optional<FileUploadBuilder> Avatar { get; set; }
}