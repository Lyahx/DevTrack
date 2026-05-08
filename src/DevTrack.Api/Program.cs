using System.Text;
using DevTrack.Api.Auth;
using DevTrack.Api.BackgroundServices;
using DevTrack.Api.Middleware;
using DevTrack.Infrastructure;
using DevTrack.Infrastructure.Auth;
using DevTrack.Infrastructure.Data;
using DevTrack.Infrastructure.Mapping;
using DevTrack.Repository;
using DevTrack.Service;
using DevTrack.Service.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((ctx, services, lc) => lc
        .ReadFrom.Configuration(ctx.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .WriteTo.File("logs/devtrack-.log", rollingInterval: RollingInterval.Day));

    var jwtSection = builder.Configuration.GetSection(JwtSettings.SectionName);
    var jwtSettings = jwtSection.Get<JwtSettings>() ?? new JwtSettings();

    builder.Services
        .AddDevTrackInfrastructure(builder.Configuration)
        .AddDevTrackRepositories()
        .AddDevTrackServices(builder.Configuration);

    builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());
    builder.Services.AddHttpContextAccessor();
    builder.Services.AddScoped<ICurrentUser, CurrentUser>();

    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false;
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),
                ClockSkew = TimeSpan.FromSeconds(30)
            };
        });

    builder.Services.AddAuthorization(options =>
    {
        options.FallbackPolicy = new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .Build();
    });

    builder.Services.AddControllers()
        .AddJsonOptions(o =>
        {
            o.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
            o.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
        });

    builder.Services.AddOpenApi();

    var allowedOrigins = (builder.Configuration["Cors:AllowedOrigins"]
            ?? Environment.GetEnvironmentVariable("DEVTRACK_CORS_ORIGINS")
            ?? "http://localhost:3000")
        .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

    builder.Services.AddCors(options =>
    {
        options.AddDefaultPolicy(policy =>
            policy.WithOrigins(allowedOrigins)
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials());
    });

    builder.Services.AddHostedService<ReminderGeneratorBackgroundService>();

    if (builder.Environment.IsDevelopment())
    {
        builder.Services.AddScoped<DevTrack.Service.Interfaces.IDevSeedService, DevTrack.Service.Implementations.DevSeedService>();
    }

    var app = builder.Build();

    app.UseSerilogRequestLogging();
    app.UseCors();
    app.UseMiddleware<GlobalExceptionMiddleware>();

    app.MapOpenApi().AllowAnonymous();
    app.MapScalarApiReference(options =>
    {
        options
            .WithTitle("DevTrack API")
            .WithTheme(ScalarTheme.BluePlanet)
            .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
    }).AllowAnonymous();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<DevTrackDbContext>();
        if ((Environment.GetEnvironmentVariable("DEVTRACK_AUTO_MIGRATE") ?? "true").Equals("true", StringComparison.OrdinalIgnoreCase))
        {
            await db.Database.MigrateAsync();
        }
    }

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly.");
}
finally
{
    Log.CloseAndFlush();
}
