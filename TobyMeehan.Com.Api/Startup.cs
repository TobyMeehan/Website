using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualBasic.CompilerServices;
using MySql.Data.MySqlClient;
using TobyMeehan.Com.Api.Authorization;
using TobyMeehan.Com.Api.Models;
using TobyMeehan.Com.Api.Models.Api;
using TobyMeehan.Com.Api.Models.OAuth;
using TobyMeehan.Com.AspNetCore;
using TobyMeehan.Com.AspNetCore.Authentication;
using TobyMeehan.Com.Data.Configuration;
using TobyMeehan.Com.Data.Models;
using TobyMeehan.Com.Data.Security;

namespace TobyMeehan.Com.Api
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
            var cloudConfig = Configuration.GetSection("CloudStorage");
            string storageCredential = cloudConfig.GetSection("StorageCredential").Value;
            GoogleCredential googleCredential;

            if (string.IsNullOrEmpty(storageCredential))
            {
                googleCredential = GoogleCredential.FromFile(cloudConfig.GetSection("StorageCredentialFile").Value);
            }
            else
            {
                googleCredential = GoogleCredential.FromJson(storageCredential);
            }

            var jwtConfig = Configuration.GetSection("Jwt");
            
            services.AddDataAccessLibrary()
                .AddSqlDatabase(() => new MySqlConnection(Configuration.GetConnectionString("Default")))
                .AddBCryptPasswordHash()
                .AddGoogleCloudStorage(googleCredential, options => { })
                .AddSymmetricTokenProvider(options =>
                {
                    options.Audience = jwtConfig["Audience"];
                    options.Issuer = jwtConfig["Issuer"];
                    options.Key = jwtConfig["Key"];
                });
            
            services.AddSingleton(ConfigureMapper());

            services.AddControllersWithViews();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig["Key"])),
                    ValidAudience = jwtConfig["Audience"],
                    ValidIssuer = jwtConfig["Issuer"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromSeconds(0)
                };
            });

            var keyRingConfig = Configuration.GetSection("KeyRing");
            string keyRingBucket = keyRingConfig.GetSection("BucketName").Value;
            string dataProtectionObject = keyRingConfig.GetSection("DataProtection").Value;

            services.AddSharedCookieAuthentication(keyRingBucket, dataProtectionObject, options =>
            {
                Func<HttpContext, string> getReturnUrl = context => $"?ReturnUrl={WebUtility.UrlEncode($"{context.Request.Host}{context.Request.Path}")}";

                options.Events = new CookieAuthenticationEvents
                {
                    OnRedirectToLogin = context =>
                    {
#if DEBUG
                        context.HttpContext.Response.Redirect($"https://localhost:44373/login{getReturnUrl.Invoke(context.HttpContext)}");
#else
                        context.HttpContext.Response.Redirect($"https://tobymeehan.com/login{getReturnUrl.Invoke(context.HttpContext)}");
#endif
                        return Task.CompletedTask;
                    },
                    OnRedirectToAccessDenied = context =>
                    {
#if DEBUG
                        context.HttpContext.Response.Redirect($"https://localhost:44373/login{getReturnUrl.Invoke(context.HttpContext)}");
#else
                        context.HttpContext.Response.Redirect($"https://tobymeehan.com/login{getReturnUrl.Invoke(context.HttpContext)}");
#endif
                        return Task.CompletedTask;
                    }
                };
            });

            services.AddScopeAuthorization();

            services.AddAuthorization(options =>
            {
                AuthorizationPolicy jwt = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser()
                    .Build();

                AuthorizationPolicy cookies = new AuthorizationPolicyBuilder(CookieAuthenticationDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser()
                    .Build();

                options.AddPolicy("jwt", jwt);
                options.AddPolicy("cookies", cookies);

                options.DefaultPolicy = jwt;
            });

            services.AddSingleton<IAuthorizationHandler, UserAuthorizationHandler>();
        }

        private IMapper ConfigureMapper()
        {
            return new MapperConfiguration(cfg =>
            {
                cfg.CreateMap(typeof(Data.Collections.EntityCollection<>), typeof(Api.Models.EntityCollection<>));
                cfg.CreateMap<EntityBase, EntityModel>().ReverseMap();
                cfg.CreateMap<Connection, ConnectionModel>().ReverseMap();
                cfg.CreateMap<Application, ApplicationModel>().ReverseMap();
                cfg.CreateMap<User, UserModel>().ReverseMap();
                cfg.CreateMap<Role, RoleModel>().ReverseMap();
                cfg.CreateMap<Download, DownloadModel>().ReverseMap();
                cfg.CreateMap<DownloadFile, DownloadFileModel>().ReverseMap();


                cfg.CreateMap<WebToken, JsonWebTokenResponse>().ReverseMap();

                cfg.CreateMap<Application, ApplicationResponse>();
                cfg.CreateMap<ApplicationModel, ApplicationResponse>();

                cfg.CreateMap<DownloadRequest, Download>()
                    .ForMember(
                        dest => dest.Version,
                        opt => opt.MapFrom(src => Version.Parse(src.Version)));
                cfg.CreateMap<DownloadRequest, DownloadModel>()
                    .ForMember(
                        dest => dest.Version,
                        opt => opt.MapFrom(src => Version.Parse(src.Version)));

                cfg.CreateMap<Download, DownloadResponse>()
                    .ForMember(
                        dest => dest.Version,
                        opt => opt.MapFrom(src => src.VersionString));
                cfg.CreateMap<DownloadModel, DownloadResponse>()
                    .ForMember(
                        dest => dest.Version,
                        opt => opt.MapFrom(src => src.Version.ToString()));

                cfg.CreateMap<DownloadFile, DownloadFileResponse>();
                cfg.CreateMap<DownloadFileModel, DownloadFileResponse>();

                cfg.CreateMap<Objective, ObjectiveResponse>();
                cfg.CreateMap<Score, ScoreResponse>();

                cfg.CreateMap<User, UserResponse>();
                cfg.CreateMap<UserModel, UserResponse>();
                cfg.CreateMap<User, PartialUserResponse>();
                cfg.CreateMap<UserModel, PartialUserResponse>();

                cfg.CreateMap<Role, RoleResponse>();
                cfg.CreateMap<RoleModel, RoleResponse>();
                cfg.CreateMap<Transaction, TransactionResponse>();
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
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
