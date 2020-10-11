using System;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Skeptical.Beavers.Backend.Challenges;
using Skeptical.Beavers.Backend.Configurations;
using Skeptical.Beavers.Backend.Obfuscation;
using Skeptical.Beavers.Backend.Services;
using Skeptical.Beavers.Backend.Utils;

namespace Skeptical.Beavers.Backend
{
    public class Startup
    {
        public const string CorsPolicy = "AllowAll";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                    options.SerializerSettings.MissingMemberHandling = MissingMemberHandling.Error;
                });;

            var npmConfig = new NpmConfig();
            Configuration.GetSection("npm").Bind(npmConfig);
            services.AddSingleton(npmConfig);

            var jwtConfig = Configuration.GetSection("jwtConfig").Get<JwtConfig>();
            services.AddSingleton(jwtConfig);
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = true;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtConfig.Issuer,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtConfig.Secret)),
                    ValidAudience = jwtConfig.Audience,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(1)
                };
            });

            services
                .AddSingleton<IJwtAuthManager, JwtAuthManager>()
                .AddSingleton<IUserService, UserService>()
                .AddSingleton<IAppsService, AppsService>()
                .AddSingleton<IChallengeRepository, ChallengeRepository>()
                .AddSingleton<ObfuscatedEndpointsRepository>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Sceptical Beavers Example API",
                    Version = "v1",
                    Contact = new OpenApiContact
                    {
                        Name = "Nikita Chizhov",
                        Email = "nik96a@gmail.com"
                    },
                    Description = "Based on https://github.com/dotnet-labs/JwtAuthDemo"
                });

                var securityScheme = new OpenApiSecurityScheme
                {
                    Name = "JWT Authentication",
                    Description = "Enter JWT Bearer token **_only_**",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Reference = new OpenApiReference
                    {
                        Id = JwtBearerDefaults.AuthenticationScheme,
                        Type = ReferenceType.SecurityScheme
                    }
                };
                c.AddSecurityDefinition(securityScheme.Reference.Id, securityScheme);
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {securityScheme, new string[] { }}
                });
            });

            services.AddCors(options =>
            {
                options.AddPolicy(CorsPolicy,
                    builder => { builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader(); });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMiddleware<EndpointDeobfuscationMiddleware>();
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseCors(CorsPolicy);
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("v1/swagger.json", "Sceptical Beavers API Example Swagger v1");
                c.DocumentTitle = "Sceptical Beavers API Example Swagger";
                c.RoutePrefix = "swagger";
            });

        }
    }
}