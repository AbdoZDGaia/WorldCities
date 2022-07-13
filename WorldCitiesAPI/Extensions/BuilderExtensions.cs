using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;
using WorldCitiesAPI.Data;
using WorldCitiesAPI.Data.GraphQL;
using WorldCitiesAPI.Data.Models;

namespace WorldCitiesAPI.Extensions
{
    public static class BuilderExtensions
    {
        public static void ConfigureServices(this WebApplicationBuilder builder)
        {
            // Adds Serilog support
            AddSerilog(builder);

            // Add services to the container.
            ConfigureControllers(builder);

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            AddSwagger(builder);

            // Add CORS
            AddCors(builder);

            // Add ApplicationDbContext and SQL Server support
            AddSqlDbContext(builder);

            // Add ASP.NET Core Identity support
            AddIdentity(builder);

            // Add Authentication services & middlewares
            AddAuthentication(builder);

            // Add services to the DI container
            AddServices(builder);

            // Add AutoMapper
            AddAutoMapper(builder);

            // Add GraphQL
            AddGraphQL(builder);
        }

        private static void AddGraphQL(WebApplicationBuilder builder)
        {
            builder.Services.AddGraphQLServer()
                            .AddAuthorization()
                            .AddQueryType<Query>()
                            .AddMutationType<Mutation>()
                            .AddFiltering()
                            .AddSorting();
        }

        private static void AddAutoMapper(WebApplicationBuilder builder)
        {
            builder.Services.AddAutoMapper(typeof(Program));
        }

        private static void AddServices(WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<JwtHandler>();
        }

        private static void AddAuthentication(WebApplicationBuilder builder)
        {
            builder.Services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    RequireExpirationTime = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
                    ValidAudience = builder.Configuration["JwtSettings:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        System.Text.Encoding.UTF8.GetBytes(
                            builder.Configuration["JwtSettings:SecurityKey"]))
                };
            });
        }

        private static void AddIdentity(WebApplicationBuilder builder)
        {
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.SignIn.RequireConfirmedAccount = true;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredLength = 8;
            })
                            .AddEntityFrameworkStores<ApplicationDbContext>();
        }

        private static void AddSqlDbContext(WebApplicationBuilder builder)
        {
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer(
                        builder.Configuration.GetConnectionString(
                            "sqlConnection")));
        }

        private static void AddCors(WebApplicationBuilder builder)
        {
            builder.Services.AddCors(opts =>
            {
                opts.AddPolicy("CorsPolicy", builder =>
                {
                    builder.AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowAnyOrigin();
                });
            });
        }

        private static void AddSwagger(WebApplicationBuilder builder)
        {
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(x =>
            {
                x.SwaggerDoc("v1", new OpenApiInfo { Title = "World Cities API", Version = "v1" });
                x.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the bearer scheme",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                });
                x.AddSecurityRequirement(new OpenApiSecurityRequirement{
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new List<string> { }
                    }
                });
            });
        }

        private static void ConfigureControllers(WebApplicationBuilder builder)
        {
            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    // options.JsonSerializerOptions.WriteIndented = true;
                    // options.JsonSerializerOptions.PropertyNamingPolicy = null;
                });
        }

        private static void AddSerilog(WebApplicationBuilder builder)
        {
            builder.Host.UseSerilog((ctx, lc) => lc
                .ReadFrom.Configuration(ctx.Configuration)
                .WriteTo.MSSqlServer(connectionString:
                            ctx.Configuration.GetConnectionString("sqlConnection"),
                        restrictedToMinimumLevel: LogEventLevel.Information,
                        sinkOptions: new MSSqlServerSinkOptions
                        {
                            TableName = "LogEvents",
                            AutoCreateSqlTable = true
                        }
                        )
                .WriteTo.Console()
                );
        }
    }
}
