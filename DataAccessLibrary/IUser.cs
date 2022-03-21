using System.Collections.Generic;

namespace TobyMeehan.Com.Data
{
    public interface IUser
    {
        Id<IUser> Id { get; }
        
        string Username { get; }
        string Description { get; }
        
        int Balance { get; }
        
        string VanityUrl { get; }
        string ProfilePictureUrl { get; }
        
        IReadOnlyList<IRole> Roles { get; }
    }
}