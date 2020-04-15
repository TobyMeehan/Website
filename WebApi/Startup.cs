using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AutoMapper;
using DataAccessLibrary.Data;
using DataAccessLibrary.DataAccess;
using DataAccessLibrary.Storage;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WebApi.Jwt;
using WebApi.Models;
using WebApi.ResourceAuth;

namespace WebApi
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
            services.AddControllersWithViews(options =>
            {
                options.InputFormatters.Add(new FormDataInputFormatter());
            });


            var tokenProvider = new RsaJwtTokenProvider("issuer", "audience", "keyName");
            services.AddSingleton<ITokenProvider>(tokenProvider);

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = tokenProvider.GetValidationParameters();
            });

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
            {
                options.LoginPath = "/login";
                options.AccessDeniedPath = "/login";
                options.Validate();
            });

            services.AddAuthorization(auth =>
            {
                auth.AddPolicy("Cookies", new AuthorizationPolicyBuilder()
                    .AddAuthenticationSchemes(CookieAuthenticationDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser()
                    .Build());

                auth.AddPolicy("Bearer", new AuthorizationPolicyBuilder()
                    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser()
                    .Build());

                auth.AddPolicy("ApplicationPolicy", policy =>
                    policy.Requirements.Add(new ApplicationAuthorRequirement()));
            });

            services.AddSingleton<IAuthorizationHandler, ApplicationAuthorizationHandler>();

            services.AddSingleton(ConfigureMapper());

            services.AddSingleton<HttpClient>();

            services.AddTransient<ISqlDataAccess, SqlDataAccess>();
            services.AddTransient<IHttpDataAccess, HttpDataAccess>();

            services.AddTransient<IUserTable, UserTable>();
            services.AddTransient<IRoleTable, RoleTable>();
            services.AddTransient<ITransactionTable, TransactionTable>();
            services.AddTransient<IUserRoleTable, UserRoleTable>();

            services.AddTransient<IApplicationTable, ApplicationTable>();
            services.AddTransient<IConnectionTable, ConnectionTable>();
            services.AddTransient<IAuthorizationCodeTable, AuthorizationCodeTable>();
            services.AddTransient<IPkceTable, PkceTable>();

            services.AddTransient<IDownloadTable, DownloadTable>();
            services.AddTransient<IDownloadFileTable, DownloadFileTable>();
            services.AddTransient<IDownloadAuthorTable, DownloadAuthorTable>();
            services.AddTransient<IDownloadFileApi, DownloadFileApi>();

            services.AddTransient<IUserProcessor, UserProcessor>();
            services.AddTransient<IRoleProcessor, RoleProcessor>();
            services.AddTransient<IApplicationProcessor, ApplicationProcessor>();
            services.AddTransient<IConnectionProcessor, ConnectionProcessor>();
            services.AddTransient<IDownloadProcessor, DownloadProcessor>();
        }

        private IMapper ConfigureMapper()
        {
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<DataAccessLibrary.Models.User, User>().ReverseMap();
                cfg.CreateMap<DataAccessLibrary.Models.Role, Role>().ReverseMap();
                cfg.CreateMap<DataAccessLibrary.Models.Transaction, Transaction>().ReverseMap();
                cfg.CreateMap<DataAccessLibrary.Models.Application, Application>().ReverseMap();
                cfg.CreateMap<DataAccessLibrary.Models.Connection, Connection>().ReverseMap();
                cfg.CreateMap<DataAccessLibrary.Models.AuthorizationCode, AuthorizationCode>().ReverseMap();
                cfg.CreateMap<DataAccessLibrary.Models.Download, Download>().ReverseMap();
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
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            var cookiePolicyOptions = new CookiePolicyOptions
            {
                MinimumSameSitePolicy = Microsoft.AspNetCore.Http.SameSiteMode.Lax
            };

            app.UseCookiePolicy(cookiePolicyOptions);
        }
    }
}
