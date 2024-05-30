using API.Helpers;
using API.Helpers.Errors;
using API.Services;
using Asp.Versioning;
using AspNetCoreRateLimit;
using CORE.Entities;
using CORE.Interfaces;
using Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace API.Extensions;
public static class ApplicationServiceExtensions
{
    public static void ConfigureCors(this IServiceCollection services) =>
    services.AddCors(options =>
    {
        options.AddPolicy("CorsPolicy", builder =>
            builder.WithOrigins().AllowAnyMethod().AllowAnyHeader());
    });

    public static void AddAplicacionServices(this IServiceCollection services)
    {
        //services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        //services.AddScoped<IProductoRepository, ProductoRepository>();
        //services.AddScoped<IMarcaRepository, MarcaRepository>();
        //services.AddScoped<ICategoriaRepository, CategoriaRepository>();
        services.AddScoped<IPasswordHasher<Usuario>, PasswordHasher<Usuario>>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
    }

    public static void ConfigureRateLimiting(this IServiceCollection services)
    {
        services.AddMemoryCache();
        services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
        services.AddInMemoryRateLimiting();

        services.Configure<IpRateLimitOptions>(options =>
        {
            options.EnableEndpointRateLimiting = true;
            options.StackBlockedRequests = false;
            options.HttpStatusCode = 429;
            options.RealIpHeader = "X-Real-IP";
            options.GeneralRules = new List<RateLimitRule>
            {
                new RateLimitRule
                {
                    Endpoint = "*",
                    Period = "10s",
                    Limit = 2
                }
            };
        });
    }

    public static void ConfigureApiVersioning(this IServiceCollection services)
    {
        services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.AssumeDefaultVersionWhenUnspecified = true;
            //options.ApiVersionReader = new QueryStringApiVersionReader("ver");
            //options.ApiVersionReader = new HeaderApiVersionReader("ver");
            options.ApiVersionReader = ApiVersionReader.Combine(
                    new QueryStringApiVersionReader("ver"),
                    new HeaderApiVersionReader("version")
                );
            options.ReportApiVersions = true;
        }).AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'V";
            options.SubstituteApiVersionInUrl = true;
        });
    }

    public static void AddJwt(this IServiceCollection services,
       IConfiguration configuration)
    {
        //Configuration from AppSettings
        services.Configure<JWT>(configuration.GetSection("JWT"));

        //Adding Athentication - JWT
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme =
                    JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme =
                    JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false;
            options.SaveToken = false;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
                ValidIssuer = configuration["JWT:Issuer"],
                ValidAudience = configuration["JWT:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8
                                                .GetBytes(configuration["JWT:Key"]))
            };
        });
    }

    /// <summary>
    /// Método de extensión que permite manejar los errores del modelstate
    /// </summary>
    /// <param name="services"></param>
    public static void AddValidtionErrors(this IServiceCollection services)
    {
        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = actionContext =>
            {
                string[] errors = actionContext.ModelState
                                        .Where(u => u.Value.Errors.Count > 0)
                                        .SelectMany(u => u.Value.Errors)
                                        .Select(u => u.ErrorMessage).ToArray();

                ApiValidation errorResponse = new()
                {
                    Errors = errors
                };

                return new BadRequestObjectResult(errorResponse);
            };
        });
    }
}
