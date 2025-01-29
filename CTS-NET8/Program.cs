using CTS_NET8.Configurations;
using CTS_NET8.Connection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var interfaceConfig = new InterfaceConfig();

// Add services to the container.

builder.Services.AddControllers();
interfaceConfig.InitializeConfig();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Environment.EnvironmentName = interfaceConfig.Environment;
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowedOrigins", policy =>
    {
        policy.WithOrigins("https://crm-frontend-vitalea-pruebas.azurewebsites.net", "https://cts.vitalea.com.co", "http://localhost:4208")
              .WithHeaders("Content-Type")  // Solo acepta JSON
              .AllowAnyMethod();
    });
});

// Registrar Swagger solo si es entorno de desarrollo
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddSwaggerGen();
}

builder.Services.AddAuthentication(config =>
{
    config.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    config.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false; //ACTIVAR PARA HTTPS
        options.SaveToken = true;
        var secretKey = interfaceConfig.secretKeyJWT;
        var IssuerJWT = interfaceConfig.issuerJWT;
        var AudienceJWT = interfaceConfig.audienceJWT;
        var keyBytes = Encoding.UTF8.GetBytes(secretKey);
        options.TokenValidationParameters = new TokenValidationParameters
        {
            IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
            ValidateIssuerSigningKey = false,
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidIssuer = IssuerJWT,
            ValidAudience = AudienceJWT,
            ClockSkew = TimeSpan.Zero,
        };
    });

var app = builder.Build();

// Aplicar CORS al pipeline de middleware
app.UseCors("AllowedOrigins");

// Configure the HTTP request pipeline.
if (builder.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
