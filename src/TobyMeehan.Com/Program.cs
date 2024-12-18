using System.Text.Json.Serialization;
using FastEndpoints;
using FastEndpoints.Security;
using Microsoft.AspNetCore.Authorization;
using TobyMeehan.Com.Data.Configuration;
using TobyMeehan.Com.Domain.Downloads;
using TobyMeehan.Com.Security;
using TobyMeehan.Com.Security.Configuration;

using DownloadHandlers = TobyMeehan.Com.Security.Handlers.Downloads;
using CommentHandlers = TobyMeehan.Com.Security.Handlers.Comments;
using FileHandlers = TobyMeehan.Com.Security.Handlers.Files;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.Secret.json", optional: true);
builder.Configuration.AddEnvironmentVariables();

foreach (var section in builder.Configuration.GetChildren())
    switch (section.Key)
    {
        case "Data":
            builder.Services.AddDataAccessLibrary(section);
            break;
    }

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));

builder.Services.AddAuthenticationJwtBearer(options =>
{
    options.SigningKey = builder.Configuration["Jwt:Key"];
});

builder.Services.AddAuthorizationBuilder()
    .RegisterPolicies();

builder.Services.AddScoped<IAuthorizationHandler, DownloadHandlers.PublicHandler>();
builder.Services.AddScoped<IAuthorizationHandler, DownloadHandlers.OwnerHandler>();
builder.Services.AddScoped<IAuthorizationHandler, DownloadHandlers.AuthorHandler>();

builder.Services.AddScoped<IAuthorizationHandler, FileHandlers.PublicHandler>();
builder.Services.AddScoped<IAuthorizationHandler, FileHandlers.OwnerHandler>();
builder.Services.AddScoped<IAuthorizationHandler, FileHandlers.AuthorHandler>();

builder.Services.AddScoped<IAuthorizationHandler, CommentHandlers.PublicHandler>();
builder.Services.AddScoped<IAuthorizationHandler, CommentHandlers.UserHandler>();

builder.Services
    .AddFastEndpoints()
    .AddIdempotency();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.UseOutputCache();
app.UseFastEndpoints(config =>
{
    config.Errors.UseProblemDetails();
    
    config.Serializer.Options.Converters.Add(new JsonStringEnumConverter<Visibility>());
    config.Serializer.Options.Converters.Add(new JsonStringEnumConverter<Verification>());
});

app.Run();

public partial class Program;