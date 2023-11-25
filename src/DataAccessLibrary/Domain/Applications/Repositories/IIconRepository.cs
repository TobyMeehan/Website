using TobyMeehan.Com.Data.Domain.Applications.Models;

namespace TobyMeehan.Com.Data.Domain.Applications.Repositories;

public interface IIconRepository
{
    Task<int> InsertAsync(IconDto data, CancellationToken cancellationToken);
    Task<int> DeleteByApplicationAsync(string applicationId, CancellationToken cancellationToken);
}