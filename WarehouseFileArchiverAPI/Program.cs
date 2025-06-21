using System.Security.Claims;
using System.Text;
using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using WarehouseFileArchiverAPI.Contexts;
using WarehouseFileArchiverAPI.Interfaces;
using WarehouseFileArchiverAPI.Misc;
using WarehouseFileArchiverAPI.Models;
using WarehouseFileArchiverAPI.Policies;
using WarehouseFileArchiverAPI.Repositories;
using WarehouseFileArchiverAPI.Services;

Log.Logger = new LoggerConfiguration()
 .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", "WarehouseFileArchiverAPI")
    .WriteTo.Console()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
    .Enrich.FromLogContext()
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "Warehouse File Archive API V1", Version = "v1" });
    opt.SwaggerDoc("v2", new OpenApiInfo { Title = "Warehouse File Archive API V2", Version = "v2" });
    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });
    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});

builder.Services.AddControllers()
                .AddJsonOptions(opts =>
                {
                    opts.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
                    opts.JsonSerializerOptions.WriteIndented = true;
                });


builder.Services.AddDbContext<WarehouseDBContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

#region Repositories
builder.Services.AddTransient<IRepository<Guid, Employee>, EmployeeRepository>();
builder.Services.AddTransient<IRepository<Guid, AccessLevel>, AccessLevelRepository>();
builder.Services.AddTransient<IRepository<Guid, Category>, CategoryRepository>();
builder.Services.AddTransient<IRepository<Guid, FileArchive>, FileArchiveRepository>();
builder.Services.AddTransient<IRepository<Guid, FileVersion>, FileVersionRepository>();
builder.Services.AddTransient<IRepository<Guid, MediaType>, MediaTypeRepository>();
builder.Services.AddTransient<IRepository<Guid, Role>, RoleRepository>();
builder.Services.AddTransient<IRepository<Guid, RoleCategoryAccess>, RoleCategoryAccessRepository>();
builder.Services.AddTransient<IRepository<string, User>, UserRepository>();
builder.Services.AddTransient<IRepository<Guid, AuditLog>, AuditLogRepository>();
#endregion


#region Services
builder.Services.AddTransient<IAccessLevelService, AccessLevelService>();
builder.Services.AddTransient<IAuthenticationService, AuthenticationService>();
builder.Services.AddTransient<ICategoryService, CategoryService>();
builder.Services.AddTransient<IEmployeeService, EmployeeService>();
builder.Services.AddTransient<IEncryptionService, EncryptionService>();
builder.Services.AddTransient<IFileArchiveService, FileArchiveService>();
builder.Services.AddTransient<IFileVersionService, FileVersionService>();
builder.Services.AddTransient<IMediaTypeService, MediaTypeService>();
builder.Services.AddTransient<IRoleService, RoleService>();
builder.Services.AddTransient<IRoleCategoryAccessService, RoleCategoryAccessService>();
builder.Services.AddTransient<ITokenService, TokenService>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IAuditLogService, AuditLogService>();


#endregion


#region  Misc
builder.Services.AddTransient<IChecksumService, ChecksumService>();
builder.Services.AddSingleton<ITokenLogoutService, TokenLogoutService>();
#endregion

#region Policies
builder.Services.AddAuthorization(opt =>
{
    opt.AddPolicy("UploadAccess", policy => policy.Requirements.Add(new MinimumAccessLevel("Write")));
});

builder.Services.AddSingleton<IAuthorizationHandler, MinAccessLevelHandler>();
#endregion

#region Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
                            if (!string.IsNullOrWhiteSpace(authHeader) && authHeader.StartsWith("Bearer "))
                            {
                                var token = authHeader.Split(" ").Last();
                                context.HttpContext.Items["access_token"] = token;
                            }
                            return Task.CompletedTask;
                        },
                        OnTokenValidated = context =>
                        {
                            var logoutService = context.HttpContext.RequestServices.GetRequiredService<ITokenLogoutService>();
                            var token = context.HttpContext.Items["access_token"] as string;
                            if (!string.IsNullOrEmpty(token) && logoutService.IsLoggedOut(token))
                            {
                                context.Fail("This token has been revoked.");
                            }
                            return Task.CompletedTask;
                        }
                    };
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = false,
                        ValidateIssuer = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Keys:JwtTokenKey"]))
                    };
                });

#endregion

#region RateLimiting
builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.AddInMemoryRateLimiting();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
#endregion

builder.Services.AddApiVersioning(options =>
 {
     options.DefaultApiVersion = new ApiVersion(1, 0);
     options.AssumeDefaultVersionWhenUnspecified = true;
     options.ReportApiVersions = true;
     options.ApiVersionReader = ApiVersionReader.Combine(
         new UrlSegmentApiVersionReader(),
         new HeaderApiVersionReader("X-Api-Version")
     );
 });
builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();
app.UseIpRateLimiting();

app.UseSerilogRequestLogging(options =>
{
    options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
    {
        diagnosticContext.Set("UserId", httpContext.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Anonymous");
        diagnosticContext.Set("Endpoint", httpContext.GetEndpoint()?.DisplayName ?? "Unknown");
    };
    options.MessageTemplate = "Handled {RequestPath} {RequestMethod} for {UserId}";
});

app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (Exception ex)
    {
        Log.Error(ex, "Unhandled exception for {Path}", context.Request.Path);
        context.Response.StatusCode = 500;
        await context.Response.WriteAsync("An unexpected error occurred.");
    }
});

app.MapControllers();


app.Run();
