using TobyMeehan.Com.Models.User;

namespace TobyMeehan.Com.Builders.User;

/// <summary>
/// Builder structure used to create a new user.
/// </summary>
public struct CreateUserBuilder : ICreateUser
{
    /// <summary>
    /// Sets the <see cref="Username"/> property of the builder.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public CreateUserBuilder WithUsername(string value) => this with { Username = value };
    
    /// <summary>
    /// The username of the user.
    /// </summary>
    public string Username { get; set; }
    
    /// <summary>
    /// Sets the <see cref="Password"/> property of the builder.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public CreateUserBuilder WithPassword(Password value) => this with { Password = value };
    
    /// <summary>
    /// The password, to be hashed, of the user.
    /// </summary>
    public Password Password { get; set; }
}