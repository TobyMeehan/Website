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
using TobyMeehan.Com.Data.Extensions;
using TobyMeehan.Com.Hubs;
using Microsoft.AspNetCore.ResponseCompression;
using System.Net.Mime;

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
            services.AddDataAccessLibrary(options =>
            {

                options.ConnectionFactory = () => new MySqlConnection(Configuration.GetConnectionString("Default"));
                options.StorageCredential = GoogleCredential.FromFile(Configuration.GetSection("StorageCredential").Value);

                options.DownloadStorageBucket = Configuration.GetSection("DownloadStorageBucket").Value;
                options.ProfilePictureStorageBucket = Configuration.GetSection("ProfilePictureStorageBucket").Value;

            });

            services.AddSingleton(ConfigureMapper());

            services.AddTransient<JavaScript>();

            services.AddScoped<AlertState>();
            services.AddScoped<EditDownloadState>();
            services.AddScoped<ProgressTaskState>();

            services.AddFileReaderService();

            services.AddRazorPages();

            services.AddSignalR();
            services.AddServerSideBlazor();

            services.AddControllersWithViews();

            services.AddResponseCompression(options =>
            {
                options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[]
                {
                    MediaTypeNames.Application.Octet
                });
            });

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/login";
                    options.AccessDeniedPath = "/login";

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
            app.UseResponseCompression();

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
                endpoints.MapHub<ButtonHub>("/buttonhub");
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
