using FluentValidation;
using FluentValidation.AspNetCore;
using SqlKata.Compilers;
using TobyMeehan.Com.Accounts.Authentication;
using TobyMeehan.Com.Accounts.Models;
using TobyMeehan.Com.Data.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("secrets.json", true);
builder.Configuration.AddJsonFile($"secrets.{builder.Environment.EnvironmentName}.json", true);
builder.Configuration.AddEnvironmentVariables();

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddDataAccessLibrary(builder.Configuration.GetSection("Data"))
    .AddBase64IdGeneration()
    .AddBCryptPasswordHash()
    .AddRngSecretService()
    .AddPostgresDatabase()
    .AddSqlKataRepositories<PostgresCompiler>()
    .AddEntityServices();

builder.Services.AddHttpContextAccessor();

builder.Services.AddCookieAuthentication();

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

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();