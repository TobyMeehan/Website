using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TobyMeehan.Com.Data.Configuration;

public static class ServiceCollectionExtensions
{
    public static DataAccessLibraryBuilder AddDataAccessLibrary(this IServiceCollection services)
    {
        return new DataAccessLibraryBuilder(services);
    }

    public static DataAccessLibraryBuilder AddDataAccessLibrary(this IServiceCollection services,
        IConfiguration configuration)
    {
        return new DataAccessLibraryBuilder(services, configuration);
    }
}