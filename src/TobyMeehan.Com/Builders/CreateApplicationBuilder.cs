namespace TobyMeehan.Com.Builders;

/// <summary>
/// Builder structure used to create a new application.
/// </summary>
public struct CreateApplicationBuilder
{
    /// <summary>
    /// Sets the <see cref="Author"/> property of the builder.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public CreateApplicationBuilder WithAuthor(Id<IUser> value) => this with { Author = value };
    
    /// <summary>
    /// The author of the application.
    /// </summary>
    public Id<IUser> Author { get; set; }

    /// <summary>
    /// Sets the <see cref="Name"/> property of the builder.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public CreateApplicationBuilder WithName(string value) => this with { Name = value };
    
    /// <summary>
    /// The name of the application.
    /// </summary>
    public string Name { get; set; }
}