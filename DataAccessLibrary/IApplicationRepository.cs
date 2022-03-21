using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace TobyMeehan.Com.Data;

public interface IApplicationRepository
{
    Task<IReadOnlyList<IApplication>> GetAsync();

    Task<IApplication> GetByIdAsync(string id);

    Task<IReadOnlyList<IApplication>> GetByUserAsync(Id<IUser> userId);

    Task<IApplication> AddAsync(Action<NewApplication> app);

    Task<IApplication> UpdateAsync(Id<IApplication> id, Action<EditApplication> app);

    Task<string> AddIconAsync(Id<IApplication> id, string filename, string contentType, Stream fileStream,
        CancellationToken cancellationToken = default);

    Task RemoveIconAsync(Id<IApplication> id);

    Task DeleteAsync(Id<IApplication> id);
    
    Task<bool> ValidateAsync(string id, string secret, string redirectUri, bool ignoreSecret);
}

public class NewApplication
{
    public Id<IUser>? UserId { get; set; } = null;
    public string Name { get; set; } = null;
    public string RedirectUri { get; set; } = null;
    public bool? Secret { get; set; } = null;
}

public class EditApplication
{
    public string Name { get; set; } = null;
    public string Description { get; set; } = null;
    public string RedirectUri { get; set; } = null;
    public string Secret { get; set; } = null;
}