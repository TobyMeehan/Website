using System.Security.Cryptography.X509Certificates;
using FastEndpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenIddict()
    .AddValidation(options =>
    {
        options.SetIssuer(builder.Configuration["OIDC:Issuer"] ??
                          throw new Exception("OIDC issuer not provided."));

        options.AddEncryptionCertificate(new X509Certificate2(Convert.FromBase64String(
            builder.Configuration["OIDC:EncryptionCertificate"] ??
            throw new Exception("OIDC encryption certificate not provided."))));
        
        options.UseSystemNetHttp();
        
        options.AddEncryptionCertificate(
            new X509Certificate2(builder.Configuration["OIDC:EncryptionCertificateFile"] ?? 
                                 throw new Exception("Encryption certificate not provided.")));
    });

builder.Services.AddFastEndpoints();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.UseFastEndpoints(options =>
{
    options.Errors.UseProblemDetails();

    options.Serializer.Options.PropertyNamingPolicy = new JsonSnakeCaseNamingPolicy();
    options.Serializer.Options.Converters.Add(new OptionalConverterFactory());
});

app.Run();