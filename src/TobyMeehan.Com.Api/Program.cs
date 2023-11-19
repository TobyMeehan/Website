using System.Security.Cryptography.X509Certificates;
using FastEndpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenIddict()
    .AddValidation(options =>
    {
        options.SetIssuer(builder.Configuration["OIDC:AuthorizationServerHost"] ??
                          throw new Exception("Authorization server host not provided."));
        
        options.UseSystemNetHttp();
        
        options.AddEncryptionCertificate(
            new X509Certificate2(builder.Configuration["OIDC:EncryptionCertificateFile"] ?? 
                                 throw new Exception("Encryption certificate not provided.")));
    });

builder.Services.AddFastEndpoints();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.UseFastEndpoints();

app.Run();