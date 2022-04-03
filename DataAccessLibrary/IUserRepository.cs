using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Upload;

namespace TobyMeehan.Com.Data;

public interface IUserRepository
{
    Task<IReadOnlyList<IUser>> GetAsync();

    Task<IReadOnlyList<IUser>> GetByRoleAsync(string name);
    
    Task<IReadOnlyList<IUser>> GetByDownloadAsync(Id<IDownload> downloadId);

    Task<IUser> GetByIdAsync(Id<IUser> id);

    Task<IUser> GetByUsernameAsync(string username);

    Task<IUser> GetByVanityUrlAsync(string url);

    Task<(bool Authenticated, IUser User)> AuthenticateAsync(string username, string password);

    Task<IUser> AddAsync(Action<NewUser> user);

    Task<IUser> UpdateAsync(Id<IUser> id, Action<EditUser> user);

    Task UpdatePasswordAsync(Id<IUser> id, string password);

    Task AddRoleAsync(Id<IUser> id, Id<IRole> roleId);

    Task RemoveRoleAsync(Id<IUser> id, Id<IRole> roleId);

    Task AddAvatarAsync(Id<IUser> id, string filename, string contentType, Stream fileStream,
        CancellationToken cancellationToken = default, IProgress<IUploadProgress> progress = null);

    Task RemoveAvatarAsync(Id<IUser> id);

    Task<ITransaction> AddTransactionAsync(Id<IUser> id, Action<NewTransaction> transaction);

    Task<(bool Exists, Id<IUser> Id)> ExistsByUsernameAsync(string username);

    Task<(bool Exists, Id<IUser> Id)> ExistsByVanityUrlAsync(string url);

    Task DeleteAsync(Id<IUser> id);
}

public class EditUser
{
    public string Username { get; set; } = null;
    public string Description { get; set; } = null;
    public string VanityUrl { get; set; } = null;
}

public class NewUser
{
    public string Username { get; set; } = null;
    public string Password { get; set; } = null;
}