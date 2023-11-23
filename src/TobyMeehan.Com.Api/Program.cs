using System.Security.Cryptography.X509Certificates;
using System.Text.Json.Serialization;
using FastEndpoints;
using JorgeSerrano.Json;
using Microsoft.IdentityModel.Logging;
using OpenIddict.Validation.AspNetCore;
using TobyMeehan.Com.Api;
using TobyMeehan.Com.Api.Security;
using TobyMeehan.Com.Data.Configuration;

IdentityModelEventSource.ShowPII = true;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("secrets.json", true);
builder.Configuration.AddJsonFile($"secrets.{builder.Environment.EnvironmentName}.json", true);
builder.Configuration.AddEnvironmentVariables();

builder.Services.AddAuthentication(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);

builder.Services.AddSecurityPolicies();

builder.Services.AddMemoryCache();

builder.Services.AddDataAccessLibrary(builder.Configuration.GetSection("Data"))
    .AddRngSecretService()
    .AddBCryptPasswordHash()
    .AddBase64IdGeneration()
    .AddPostgresDatabase()
    .AddSqlKataRepositories()
    .AddEntityServices()
    .AddGoogleCloudStorage();

builder.Services.AddOpenIddict()
    .AddValidation(options =>
    {
        options.SetIssuer(builder.Configuration["OIDC:Issuer"] ??
                          throw new Exception("OIDC issuer not provided."));

        options.AddEncryptionCertificate(new X509Certificate2(Convert.FromBase64String(
            builder.Configuration["OIDC:EncryptionCertificate"] ??
            throw new Exception("OIDC encryption certificate not provided."))));
        
        options.UseSystemNetHttp();

        options.UseAspNetCore();
    });

builder.Services.AddFastEndpoints();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.UseFastEndpoints(options =>
{
    options.Errors.UseProblemDetails();

    options.Serializer.Options.PropertyNamingPolicy = new JsonSnakeCaseNamingPolicy();
    options.Serializer.Options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault;
    options.Serializer.Options.Converters.Add(new OptionalConverterFactory());
});

app.Run();