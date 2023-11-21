using System.Security.Cryptography.X509Certificates;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.IdentityModel.Tokens;
using TobyMeehan.Com;
using TobyMeehan.Com.Accounts.Authentication;
using TobyMeehan.Com.Accounts.Models.Authentication.Login;
using TobyMeehan.Com.Accounts.Models.Authentication.Register;
using TobyMeehan.Com.Accounts.Models.OpenId;
using TobyMeehan.Com.Accounts.OpenId;
using TobyMeehan.Com.Data.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("secrets.json", true);
builder.Configuration.AddJsonFile($"secrets.{builder.Environment.EnvironmentName}.json", true);
builder.Configuration.AddEnvironmentVariables();

builder.Services.AddMvc();

builder.Services.AddOpenIddict()

    .AddCore(options =>
    {
        options.AddApplicationStore<OpenIdApplicationStore>()
            .SetDefaultApplicationEntity<OpenIdApplication>()
            .ReplaceApplicationManager<OpenIdApplicationManager>();

        options.AddAuthorizationStore<OpenIdAuthorizationStore>()
            .SetDefaultAuthorizationEntity<OpenIdAuthorization>();

        options.AddTokenStore<OpenIdTokenStore>()
            .SetDefaultTokenEntity<OpenIdToken>();

        options.AddScopeStore<OpenIdScopeStore>()
            .SetDefaultScopeEntity<OpenIdScope>();
    })

    .AddServer(options =>
    {
        options.RegisterScopes(ScopeNames.GetAll(includeGroup: true).ToArray());
        
        options.SetAuthorizationEndpointUris("oauth/authorize")
            .SetTokenEndpointUris("oauth/token")
            .SetLogoutEndpointUris("connect/logout")
            .SetUserinfoEndpointUris("connect/userinfo")
            .SetIssuer(builder.Configuration["OIDC:Issuer"] ??
                       throw new Exception("OIDC issuer not provided."));

        options.AllowAuthorizationCodeFlow()
            .AllowClientCredentialsFlow()
            .AllowRefreshTokenFlow();

        options.AddEncryptionCertificate(new X509Certificate2(Convert.FromBase64String(
            builder.Configuration["OIDC:EncryptionCertificate"] ??
            throw new Exception("OIDC encryption certificate not provided."))));

        options.AddSigningCertificate(new X509Certificate2(Convert.FromBase64String(
            builder.Configuration["OIDC:SigningCertificate"] ??
            throw new Exception("OIDC signing certificate not provided."))));

        options.UseAspNetCore()
            .EnableAuthorizationEndpointPassthrough()
            .DisableTransportSecurityRequirement();
    })
    
    .AddValidation(options =>
    {
        options.UseLocalServer();
        options.UseAspNetCore();
    });

builder.Services.AddDataAccessLibrary(builder.Configuration.GetSection("Data"))
    .AddBase64IdGeneration()
    .AddBCryptPasswordHash()
    .AddRngSecretService()
    .AddPostgresDatabase()
    .AddSqlKataRepositories()
    .AddEntityServices();

builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookieAuthentication();

builder.Services.AddFluentValidationClientsideAdapters();

builder.Services.AddScoped<IValidator<LoginFormModel>, LoginFormModel.Validator>();
builder.Services.AddScoped<IValidator<RegisterFormModel>, RegisterFormModel.Validator>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();