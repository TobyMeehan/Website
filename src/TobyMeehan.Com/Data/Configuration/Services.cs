using Microsoft.EntityFrameworkCore;
using TobyMeehan.Com.Data.DataAccess;
using TobyMeehan.Com.Data.Repositories;
using TobyMeehan.Com.Data.Services;
using TobyMeehan.Com.Domain.Downloads.Services;

namespace TobyMeehan.Com.Data.Configuration;

public static class Services
{
    public static IServiceCollection AddDataAccessLibrary(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<IDownloadService, DownloadService>();
        services.AddScoped<IDownloadAuthorService, DownloadAuthorService>();
        services.AddScoped<ICommentService, CommentService>();
        
        foreach (var section in configuration.GetChildren())
            switch (section.Key)
            {
                case "Postgres":
                    services.AddDbContext<ApplicationDbContext>(options =>
                    {
                        options.UseNpgsql(section["ConnectionString"]);
                    });

                    services.AddScoped<IDownloadRepository, Repositories.EntityFramework.DownloadRepository>();
                    services.AddScoped<IDownloadAuthorRepository, Repositories.EntityFramework.DownloadAuthorRepository>();
                    services.AddScoped<ICommentRepository, Repositories.EntityFramework.CommentRepository>();
                    
                    break;
                
                case "S3":
                    services.AddSingleton<IAmazonS3>(_ => new AmazonS3Client(
                        new BasicAWSCredentials(section["AccessKey"], section["SecretKey"]),
                        new AmazonS3Config { RegionEndpoint = RegionEndpoint.GetBySystemName(section["Region"]) }
                    ));
                    services.AddSingleton<IStorageService, S3StorageService>();
                    services.Configure<StorageOptions>(section);
                    
                    break;
            }

        return services;
    }
}