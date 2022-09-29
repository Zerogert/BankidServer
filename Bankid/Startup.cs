using AutoMapper;
using Bankid.Data;
using Bankid.Interfaces;
using Bankid.Models.Entities;
using Bankid.Models.Options;
using Bankid.Services;
using Common.BusinessLayer.Models.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bankid {
    public class Startup {
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            services.AddSingleton(provider => new MapperConfiguration(cfg => { }).CreateMapper());

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 4;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
            });

            services.AddScoped<ICurrentUser, CurrentUserResolver>();


            services.AddIdentityCore<User>()
                .AddRoles<Role>()
                .AddClaimsPrincipalFactory<UserClaimsPrincipalFactory<User, Role>>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddSignInManager()
                .AddDefaultTokenProviders();

            services.AddScoped<IUsersService, UsersService>();
            services.AddHostedService<DataInitializer>();
            services.AddDbContext<AppDbContext>();

            services.AddSwaggerGen(c => {
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme {
                    In = ParameterLocation.Header,
                    Description = "Please insert JWT with Bearer into field",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement {
                    {
                        new OpenApiSecurityScheme {
                            Reference = new OpenApiReference {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] { }
                    }
                });
                c.SwaggerDoc("v1",
                    new OpenApiInfo {
                        Title = "Bankid",
                        Version = "v1"
                    }
                );
                var filePath = Path.Combine(System.AppContext.BaseDirectory, "Bankid.xml");
                c.IncludeXmlComments(filePath);
            });
            services.AddSwaggerGenNewtonsoftSupport();

            var tokenOptions = Configuration.GetSection("AuthTokenOptions").Get<AuthTokenOptions>();

            var googleOptions = Configuration.GetSection("AuthProviders:Google").Get<BaseAuthProviderOptions>();

            var authBuilder = services
                .AddAuthentication(x => {
                    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddCookie()
                .AddJwtBearer(x => {
                    x.RequireHttpsMetadata = true;
                    x.SaveToken = true;
                    x.TokenValidationParameters = new TokenValidationParameters {
                        ValidateIssuer = true,
                        ValidIssuer = tokenOptions.Issuer,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(tokenOptions.SecretKey)),
                        ValidAudience = tokenOptions.Audience,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.FromMinutes(tokenOptions.LifetimeMinutes)

                    };
                    x.Events = new JwtBearerEvents {
                        OnAuthenticationFailed = context => {
                            var authorizationKey = context.Request.Headers.Keys.FirstOrDefault(x =>
                                string.Equals(x, SessionTokenService.AUTHORIZATION_KEY, StringComparison.CurrentCultureIgnoreCase));
                            if (authorizationKey == null) return Task.CompletedTask;

                            var accessToken = context.Request.Headers[authorizationKey];
                            var handler = new JwtSecurityTokenHandler();
                            var jwtSecurityToken = handler.ReadToken(accessToken.ToString().Substring(7)) as JwtSecurityToken;
                            if ((DateTime.UtcNow - jwtSecurityToken.ValidTo).Minutes > tokenOptions.LifetimeMinutes) return Task.CompletedTask;

                            var tokenService = context.HttpContext.RequestServices.GetRequiredService<ISessionTokenService>();
                            var refreshedSecurityToken = tokenService.GenerateJwtToken(jwtSecurityToken.Claims);
                            context.Response.Headers[authorizationKey] = $"bearer {handler.WriteToken(refreshedSecurityToken)}";
                            context.Response.Headers[SessionTokenService.AUTHORIZATION_EXPIRES_KEY] = refreshedSecurityToken.ValidTo.ToString();

                            return Task.CompletedTask;
                        }
                    };
                })
                .AddGoogle(options => {
                    options.ClientId = googleOptions.ClientId;
                    options.ClientSecret = googleOptions.ClientSecret;
                    options.SignInScheme = IdentityConstants.ExternalScheme;
                    options.SaveTokens = true;
                });
            authBuilder.AddIdentityCookies();

            var dbConfiguration = Configuration.GetSection(nameof(DbConfiguration)).Get<DbConfiguration>();
            services.AddSingleton<DbConfiguration>(dbConfiguration);

            var instanseOptions = Configuration.GetSection(nameof(InstanseOptions)).Get<InstanseOptions>();
            services.AddSingleton<InstanseOptions>(instanseOptions);

            var authTokenOptions = Configuration.GetSection(nameof(AuthTokenOptions)).Get<AuthTokenOptions>();
            services.AddSingleton<AuthTokenOptions>(authTokenOptions);
            services.AddScoped<ISessionTokenService, SessionTokenService>();

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseCors(c => {
                c.AllowAnyOrigin();
                c.AllowAnyMethod();
                c.AllowAnyHeader();
                c.WithExposedHeaders(SessionTokenService.AUTHORIZATION_KEY, SessionTokenService.AUTHORIZATION_EXPIRES_KEY);
            });
            app.UseForwardedHeaders(new ForwardedHeadersOptions {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseAuthentication();
            app.UseRouting();


            app.UseAuthorization();

            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
            });
        }
    }
}
