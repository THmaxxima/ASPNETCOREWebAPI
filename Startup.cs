using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using WebApi.Helpers;
using WebApi.Services;
using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using System;
using WebApi.Entities;
using Microsoft.Extensions.DependencyInjection.Extensions;
using static Microsoft.AspNetCore.Hosting.Internal.HostingApplication;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Converters;
using Microsoft.AspNetCore.Mvc.Formatters;
using Newtonsoft.Json;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using System.Net.Http.Headers;

namespace WebApi
{
    public class Startup
    {

        public Startup(IConfiguration configuration)
        {
           Configuration = configuration;
            //RowUserID = userId;
        }

        public IConfiguration Configuration { get; }
        public string RowUserID { get; set; }
        public string userId { get;  set; }

       
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddCors();
            
            
            //services.AddMvc().AddJsonOptions(options =>
            //{
            //    options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
            //    options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Unspecified;

            //}
            //);
            //        services.Configure<RequestLocalizationOptions>(                     opts =>
            //                            {
            //                                var supportedCultures = new[]
            //                                {

            //            new CultureInfo("th-TH"),
            //                                };

            //                                opts.DefaultRequestCulture = new RequestCulture("th-TH");
            //                                // Formatting numbers, dates, etc.
            //                                opts.SupportedCultures = supportedCultures;
            //                                // UI strings that we have localized.
            //                                opts.SupportedUICultures = supportedCultures;
            //                            });

                 
            

            services.AddDbContext<DataContext>(options =>
            {
                options.UseSqlServer(MyHelper.GetDBConn());
            });

            services
        .AddMvcCore(options =>
        {
            options.RequireHttpsPermanent = true; // does not affect api requests
            options.RespectBrowserAcceptHeader = true; // false by default

        });
            services.AddMvc();
            services.AddAutoMapper();
            // configure strongly typed settings objects
            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);
           
            // configure jwt authentication
            var appSettings = appSettingsSection.Get<AppSettings>();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);

            services.AddAuthentication(x =>
            {
                //x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            //.AddJwtBearer("<authenticationScheme>", configureOptions =>
            //{
            //    x.TokenValidationParameters.ValidateActor = false;
            //    x.TokenValidationParameters.ValidateAudience = false;
            //    x.TokenValidationParameters.ValidateIssuerSigningKey = false;
            //},
            .AddJwtBearer(x =>
            {

                //x.TokenValidationParameters.ValidateActor = false;
                //x.TokenValidationParameters.ValidateAudience = false;
                //x.TokenValidationParameters.ValidateIssuerSigningKey = true;

                x.Events = new JwtBearerEvents
                {

                    OnTokenValidated = context =>
                        {
                        //var userService = context.HttpContext.RequestServices.GetRequiredService<IUserService>();
                        //var userId = (x.TokenValidationParameters.TokenReader.ToString());

                        //userId = context.HttpContext.User.FindFirst(i => i.Type == "userId").Value;

                        //var userId = 1;
                        //var user = userService.GetById(userId);
                        //var user = "User";
                        //var userId = context.HttpContext.User.FindFirst(i => i.Type == "userId").Value;
                        //if (userId == null)
                        //{
                        //    // return unauthorized if user no longer exists
                        //    context.Fail("Unauthorized");

                        //}
                        return Task.CompletedTask;
                        }
            };

                //x.RequireHttpsMetadata = false;
                //x.SaveToken = true;


                x.TokenValidationParameters = new TokenValidationParameters
                {
                    //    ValidateIssuerSigningKey = true,

                    //    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = false,
                    ValidateIssuerSigningKey = false
                };

            });


            // configure DI for application services
            services.AddScoped<IUserService, UserService>();
            
        }

      
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            //loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            //loggerFactory.AddDebug();

            // global cors policy
            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials());

            app.UseAuthentication();

            var options = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization(options.Value);

            app.UseMvc();
        }
    }

    
}
