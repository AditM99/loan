using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using LoanOrigination.Api.Data;
using LoanOrigination.Api.Services;
using LoanOrigination.Api.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.WriteIndented = true;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection") ?? "Host=localhost;Database=loan_origination;Username=postgres;Password=postgres"));

builder.Services.AddScoped<JwtService>();
builder.Services.AddSingleton<AiEligibilityService>(); // singleton holds trained model
builder.Services.AddScoped<DocumentExtractionService>();

// CORS for Vite dev
builder.Services.AddCors(opt =>
{
    opt.AddPolicy("allow-frontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Auth
var jwtKey = builder.Configuration["Jwt:Key"] ?? "dev_secret_key_change_me";
var keyBytes = Encoding.UTF8.GetBytes(jwtKey);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Ensure DB created and seed admin
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();

    if (!db.Users.Any(u => u.Email == "admin@local"))
    {
        var admin = new User
        {
            Email = "admin@local",
            Name = "Admin",
            Role = "Admin",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123$")
        };
        db.Users.Add(admin);
        db.SaveChanges();
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles(); // for /uploads
app.UseCors("allow-frontend");
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
