using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace TobyMeehan.Com.Data;

public interface IApplicationRepository
{
    Task<IReadOnlyList<IApplication>> GetAsync();

    Task<IApplication> GetByIdAsync(Id<IApplication> id);

    Task<IReadOnlyList<IApplication>> GetByUserAsync(Id<IUser> userId);

    Task<IApplication> AddAsync(Action<NewApplication> app);

    Task<IApplication> UpdateAsync(Id<IApplication> id, Action<EditApplication> app);

    Task<string> AddIconAsync(Id<IApplication> id, Action<FileUpload> image,
        CancellationToken cancellationToken = default);

    Task RemoveIconAsync(Id<IApplication> id);

    Task DeleteAsync(Id<IApplication> id);
    
    Task<bool> ValidateAsync(Id<IApplication> id, string secret, string redirectUri, bool ignoreSecret);
}

public class NewApplication
{
    public Id<IUser> UserId { get; set; }
    public string Name { get; set; }
    public string RedirectUri { get; set; }
    public string Secret { get; set; }
}

public class EditApplication
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string RedirectUri { get; set; }
    public string Secret { get; set; }
}