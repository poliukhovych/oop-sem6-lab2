using ConstantTalk.Server.Data;
using Microsoft.EntityFrameworkCore;
using ConstantTalk.Server.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using ConstantTalk.Server.Middleware;

var builder = WebApplication.CreateBuilder(args);

var auth0Settings = builder.Configuration.GetSection("Auth0").Get<Auth0Settings>();

var requestLoggingSection = builder.Configuration.GetSection("RequestLogging");
builder.Services.Configure<RequestLoggingSettings>(requestLoggingSection);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
        options.Authority = $"https://{auth0Settings.Domain}/";
#pragma warning restore CS8602 // Dereference of a possibly null reference.
        options.Audience = auth0Settings.Audience;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = $"https://{auth0Settings.Domain}/",
            ValidateAudience = true,
            ValidAudience = auth0Settings.Audience,
            ValidateLifetime = true,
            //RoleClaimType = "https://constanttalk777/api/roles"
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("admin"));
    options.AddPolicy("SubscriberOnly", policy => policy.RequireRole("subscriber"));
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:5098", "http://localhost:5173")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

var app = builder.Build();

app.UseCors("AllowReactApp");

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();


app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
//app.UseMiddleware<AuthMiddleware>();
app.UseAuthorization();

app.MapControllers();

app.Run();
