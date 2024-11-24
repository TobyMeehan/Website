using TobyMeehan.Com.Security;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthorizationBuilder()
    .RegisterPolicies();

var app = builder.Build();

app.UseAuthorization();

app.Run();