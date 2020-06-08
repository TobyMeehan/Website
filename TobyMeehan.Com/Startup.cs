using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MySql.Data.MySqlClient;
using TobyMeehan.Com.Authorization;
using TobyMeehan.Com.Data;
using TobyMeehan.Com.Data.Sql;
using TobyMeehan.Com.Data.Models;
using TobyMeehan.Sql;
using AutoMapper;
using TobyMeehan.Com.Models;
using TobyMeehan.Sql.QueryBuilder;
using TobyMeehan.Com.Pages.Downloads;
using TobyMeehan.Com.Data.Repositories;
using TobyMeehan.Com.Data.Security;
using TobyMeehan.Com.Data.CloudStorage;
using Google.Apis.Auth.OAuth2;
using TobyMeehan.Com.Tasks;
using Blazor.FileReader;

namespace TobyMeehan.Com
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<Func<IDbConnection>>(x => () => new MySqlConnection(Configuration.GetConnectionString("Default")));

            services.AddTransient<ISqlTable<User>, UserTable>();
            services.AddTransient<ISqlTable<Application>, ApplicationTable>();
            services.AddTransient<ISqlTable<Connection>, ConnectionTable>();
            services.AddTransient<ISqlTable<Download>, DownloadTable>();
            services.AddTransient(typeof(ISqlTable<>), typeof(SqlTable<>));

            services.AddTransient<ICloudStorage, GoogleCloudStorage>();
            services.AddSingleton(GoogleCredential.FromFile(Configuration.GetSection("StorageCredential").Value));

            services.AddTransient<IUserRepository, SqlUserRepository>();
            services.AddTransient<IDownloadRepository, SqlDownloadRepository>();
            services.AddTransient<IDownloadFileRepository, DownloadFileRepository>();
            services.AddTransient<IConnectionRepository, SqlConnectionRepository>();

            services.AddSingleton<IPasswordHash, BCryptPasswordHash>();

            services.AddSingleton(ConfigureMapper());

            services.AddTransient<JavaScript>();

            services.AddScoped<EditDownloadState>();
            services.AddScoped<ProgressTaskState>();

            services.AddFileReaderService();

            services.AddRazorPages();

            services.AddServerSideBlazor();

            services.AddControllersWithViews();

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/login";
                    options.AccessDeniedPath = ""; // TODO:

                    options.ExpireTimeSpan = DateTimeOffset.UtcNow.AddMonths(6).Subtract(DateTimeOffset.UtcNow);
                    options.SlidingExpiration = true;
                });

            services.AddAuthorizationPolicies();
        }

        private IMapper ConfigureMapper()
        {
            return new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Download, DownloadViewModel>().ReverseMap();
            }).CreateMapper();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseAuthentication();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
