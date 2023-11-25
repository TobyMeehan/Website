using Microsoft.Extensions.Configuration;

namespace TobyMeehan.Com.Data.Storage.Configuration;

public class GoogleStorageOptions
{
    public IConfiguration? Credential { get; set; }
    public string? CredentialJson { get; set; }
}