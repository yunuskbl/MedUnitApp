using System.Text;
using System.Text.Json;
using MedUnit.Application.Interfaces;
using MedUnit.Application.Services;
using MedUnit.Infrastructure.Data;
using MedUnit.Infrastructure.Features;
using MedUnit.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RandevuAPI.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Veritabaný
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// IAppDbContext ? AppDbContext
builder.Services.AddScoped<IAppDbContext>(provider =>
    provider.GetRequiredService<AppDbContext>());

// Dependency Injection
builder.Services.AddScoped<IKullaniciService, KullaniciService>();
builder.Services.AddScoped<IRandevuService, RandevuService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IZoomService, ZoomService>();
builder.Services.AddHostedService<HatirlaticiService>();
builder.Services.AddSignalR();
builder.Services.AddHttpClient();
builder.Services.AddControllers()
    .AddJsonOptions(opt =>
        opt.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase);

// JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]!)),
            ValidateIssuer = false,
            ValidateAudience = false
        };

        // SignalR için token query
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;

                if (!string.IsNullOrEmpty(accessToken) &&
                    path.StartsWithSegments("/hubs"))
                {
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            }
        };
    });

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
        policy.WithOrigins(
            "http://localhost:4200",
            "https://med-unit-app.vercel.app",
            "https://medunit.vercel.app"
        )
        .SetIsOriginAllowedToAllowWildcardSubdomains()
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials());
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT token gir."
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

var app = builder.Build();

try
{
    app.UseSwagger();
    app.UseSwaggerUI();


    app.UseRouting();
    app.UseCors("AllowAngular");
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapGet("/health", () => Results.Ok());
    app.MapControllers();
    app.MapHub<GorusmeHub>("/hubs/gorusme");

    app.Run();
}
catch (Exception ex)
{
    Console.WriteLine($"Uygulama hatasý: {ex.Message}");
    Console.WriteLine(ex.StackTrace);
    Console.ReadLine(); // konsol kapanmasýn
}