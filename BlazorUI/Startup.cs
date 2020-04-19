using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AutoMapper;
using Blazor.FileReader;
using BlazorUI.Authorization;
using BlazorUI.Models;
using DataAccessLibrary.Data;
using DataAccessLibrary.DataAccess;
using DataAccessLibrary.Storage;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BlazorUI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;

            });

            services.AddAuthentication(
                CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/login";
                    options.ExpireTimeSpan = DateTimeOffset.UtcNow.AddMonths(6).Subtract(DateTimeOffset.UtcNow);
                });

            services.AddRazorPages();
            services.AddServerSideBlazor().AddHubOptions(options =>
            {
                options.MaximumReceiveMessageSize = 130 * 1024 * 1024;
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy(Policies.IsVerified, Policies.IsVerifiedPolicy());
                options.AddPolicy(Policies.IsAdmin, Policies.IsAdminPolicy());
                options.AddPolicy(Policies.EditDownload, Policies.EditDownloadPolicy());
            });

            services.AddFileReaderService();

            services.AddSingleton<IAuthorizationHandler, EditDownloadAuthorizationHandler>();

            services.AddHttpContextAccessor();

            HttpClient client = new HttpClient();
            //client.DefaultRequestHeaders.Accept.Clear();
            //client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            //client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/octet-stream"));

            services.AddSingleton(client);

            services.AddSingleton(ConfigureMapper());

            services.AddTransient<ISqlDataAccess, SqlDataAccess>();
            services.AddTransient<IHttpDataAccess, HttpDataAccess>();


            services.AddTransient<IUserTable, UserTable>();
            services.AddTransient<IRoleTable, RoleTable>();
            services.AddTransient<ITransactionTable, TransactionTable>();
            services.AddTransient<IUserRoleTable, UserRoleTable>();

            services.AddTransient<IApplicationTable, ApplicationTable>();
            services.AddTransient<IConnectionTable, ConnectionTable>();
            services.AddTransient<IAuthorizationCodeTable, AuthorizationCodeTable>();
            services.AddTransient<IObjectiveTable, ObjectiveTable>();
            services.AddTransient<IScoreboardTable, ScoreboardTable>();

            services.AddTransient<IDownloadTable, DownloadTable>();
            services.AddTransient<IDownloadFileTable, DownloadFileTable>();
            services.AddTransient<IDownloadAuthorTable, DownloadAuthorTable>();
            services.AddTransient<IDownloadFileApi, DownloadFileApi>();


            services.AddTransient<IUserProcessor, UserProcessor>();
            services.AddTransient<IRoleProcessor, RoleProcessor>();

            services.AddTransient<IApplicationProcessor, ApplicationProcessor>();
            services.AddTransient<IConnectionProcessor, ConnectionProcessor>();
            services.AddTransient<IScoreboardProcessor, ScoreboardProcessor>();

            services.AddTransient<IDownloadProcessor, DownloadProcessor>();

            services.AddScoped<Pages.Downloads.EditDownloadState>();
            services.AddScoped<AlertState>();
            services.AddScoped<FileUploadState>();
        }

        private IMapper ConfigureMapper()
        {
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<DataAccessLibrary.Models.User, User>().ReverseMap();
                cfg.CreateMap<DataAccessLibrary.Models.Role, Role>().ReverseMap();
                cfg.CreateMap<DataAccessLibrary.Models.Transaction, Transaction>().ReverseMap();
                cfg.CreateMap<DataAccessLibrary.Models.Alert, Alert>().ReverseMap();

                cfg.CreateMap<DataAccessLibrary.Models.Application, Application>().ReverseMap();
                cfg.CreateMap<DataAccessLibrary.Models.Connection, Connection>().ReverseMap();

                cfg.CreateMap<DataAccessLibrary.Models.Download, Download>().ReverseMap();
                cfg.CreateMap<DataAccessLibrary.Models.Download, DownloadFormModel>().ReverseMap();
                cfg.CreateMap<DownloadFormModel, Download>().ReverseMap();
            });

            var mapper = mapperConfig.CreateMapper();

            return mapper;
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
                app.UseExceptionHandler("/Error");
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
            });
        }
    }
}
