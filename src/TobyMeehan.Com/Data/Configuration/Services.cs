using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Microsoft.EntityFrameworkCore;
using TobyMeehan.Com.Data.CloudStorage;
using TobyMeehan.Com.Data.CloudStorage.S3;
using TobyMeehan.Com.Data.DataAccess;
using TobyMeehan.Com.Data.Repositories;
using TobyMeehan.Com.Data.Services;
using TobyMeehan.Com.Domain.Downloads.Services;
using TobyMeehan.Com.Domain.Thavyra.Services;

namespace TobyMeehan.Com.Data.Configuration;

public static class Services
{
    public static IServiceCollection AddDataAccessLibrary(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<IDownloadService, DownloadService>();
        services.AddScoped<IDownloadAuthorService, DownloadAuthorService>();
        services.AddScoped<IDownloadFileService, DownloadFileService>();
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
                    services
                        .AddScoped<IDownloadAuthorRepository, Repositories.EntityFramework.DownloadAuthorRepository>();
                    services.AddScoped<IDownloadFileRepository, Repositories.EntityFramework.DownloadFileRepository>();
                    services.AddScoped<ICommentRepository, Repositories.EntityFramework.CommentRepository>();

                    break;

                case "S3":

                    AWSCredentials credentials = section.GetSection("Credentials").Exists()
                        ? new BasicAWSCredentials(section["Credentials:AccessKey"], section["Credentials:SecretKey"])
                        : new AnonymousAWSCredentials();

                    var config = new AmazonS3Config();

                    if (section.GetSection("Configuration:Region").Exists())
                    {
                        config.RegionEndpoint = RegionEndpoint.GetBySystemName(section["Configuration:Region"]);
                    }

                    if (section.GetSection("Configuration:ServiceUrl").Exists())
                    {
                        config.ServiceURL = section["Configuration:ServiceUrl"];
                    }
                    
                    var client = new AmazonS3Client(credentials, config);

                    services.AddSingleton<IAmazonS3>(client);
                    services.AddSingleton<IStorageService, S3StorageService>();
                    services.Configure<StorageOptions>(section);

                    break;

                case "Thavyra":
                    services.AddHttpClient<IUserService, UserService>(options =>
                    {
                        options.BaseAddress = new Uri(section["BaseAddress"]!);
                    });

                    break;
            }

        return services;
    }
}