namespace TobyMeehan.Com.Models.Application;

public interface ICreateApplication
{
    /// <summary>
    /// The author/owner of the application.
    /// </summary>
    Id<IUser> Author { get; }
    
    /// <summary>
    /// The name of the application
    /// </summary>
    string Name { get; }
}