using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualBasic.CompilerServices;
using MySql.Data.MySqlClient;
using TobyMeehan.Com.Api.Models;
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
            services.AddDataAccessLibrary()
                .AddSqlDatabase(() => new MySqlConnection(Configuration.GetConnectionString("Default")))
                .AddBCryptPasswordHash();

            var tokenProvider = new RsaTokenProvider("api.tobymeehan.com", "OAuth Application", Guid.NewGuid().ToString().ToUpperInvariant());
            services.AddSingleton<ITokenProvider>(tokenProvider);

            services.AddSingleton(ConfigureMapper());

            services.AddControllersWithViews();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = tokenProvider.GetValidationParameters();
            });

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
            {
#if DEBUG
                options.Cookie.Domain = "localhost";
#else
                options.Cookie.Domain = ".tobymeehan.com";
#endif

                options.Events = new CookieAuthenticationEvents
                {
                    OnRedirectToLogin = context =>
                    {
                        context.HttpContext.Response.Redirect("https://tobymeehan.com/login");
                        return Task.CompletedTask;
                    },
                    OnRedirectToAccessDenied = context =>
                    {
                        context.HttpContext.Response.Redirect("https://tobymeehan.com/login");
                        return Task.CompletedTask;
                    }
                };
            });
        }

        private IMapper ConfigureMapper()
        {
            return new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Connection, ConnectionModel>().ReverseMap();
                cfg.CreateMap<Application, ApplicationModel>().ReverseMap();
                cfg.CreateMap<User, UserModel>().ReverseMap();
                cfg.CreateMap<Role, RoleModel>().ReverseMap();
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
