namespace TobyMeehan.Com.Builders;

/// <summary>
/// Builder structure used to update protected properties of a user.
/// </summary>
public struct ProtectedUpdateUserBuilder
{
    /// <summary>
    /// Sets the <see cref="Handle"/> property of the builder.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public ProtectedUpdateUserBuilder WithHandle(string value) => this with { Handle = value };
    
    /// <summary>
    /// The user's new handle.
    /// </summary>
    public Optional<string> Handle { get; set; }

    /// <summary>
    /// Sets the <see cref="Password"/> property of the builder.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public ProtectedUpdateUserBuilder WithPassword(Password value) => this with { Password = value };
    
    /// <summary>
    /// The new password, to be hashed, of the user.
    /// </summary>
    public Optional<Password> Password { get; set; }
}