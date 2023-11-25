using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TobyMeehan.Com.Data.Configuration;
using TobyMeehan.Com.Data.Storage.Google;
using TobyMeehan.Com.Services;

namespace TobyMeehan.Com.Data.Storage.Configuration;

public static class DataAccessLibraryBuilderExtensions
{
    public static DataAccessLibraryBuilder AddGoogleCloudStorage(this DataAccessLibraryBuilder builder,
        Action<GoogleStorageOptions> configureGoogle)
    {
        var googleOptions = new GoogleStorageOptions();
        configureGoogle(googleOptions);

        return AddGoogleCloudStorage(builder, googleOptions);
    }
    
    internal static DataAccessLibraryBuilder AddGoogleCloudStorage(this DataAccessLibraryBuilder builder, 
        GoogleStorageOptions googleOptions, IConfigurationSection? credentialConfig = null)
    {
        var credential = googleOptions switch
        {
            { CredentialJson: { } json } => GoogleCredential.FromJson(json),
            
            _ when credentialConfig?.Exists() is true => 
                GoogleCredential.FromJsonParameters(credentialConfig.Get<JsonCredentialParameters>()),
            
            _ => throw new ConfigurationException<GoogleStorageOptions>(googleOptions, "No Google credential provided.")
        };

        builder.Services.AddSingleton(credential);
        
        builder. Services.AddTransient(services => 
            StorageClient.Create(services.GetRequiredService<GoogleCredential>()));
        
        return builder.AddCloudStorage<GoogleStorageService>();
    }
}