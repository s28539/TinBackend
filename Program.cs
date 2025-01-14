using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using restAPI.Repositories;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

/*CORS (Cross-Origin Resource Sharing) to mechanizm, który pozwala na kontrolowanie,
 jakie zasoby mogą być dostępne dla aplikacji działających na różnych domenach (lub originach). 
 W kontekście aplikacji webowych, CORS pozwala serwerowi określić, 
 które z zewnętrznych aplikacji (np. frontendów) mogą uzyskiwać dostęp do zasobów serwera API.*/

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        builder => builder
            .WithOrigins("http://localhost:3000") // Adres Twojego frontendowego aplikacji
            .AllowAnyMethod()
            .AllowAnyHeader());
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
        };

        
        options.TokenValidationParameters.ValidIssuer = "http://localhost:5078"; // Backend URL
        options.TokenValidationParameters.ValidAudience = "http://localhost:3000"; // Frontend URL
        
        options.TokenValidationParameters.IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("SuperSecretKey123SuperSecretKey123"));
    });

// Dodanie Swaggera dla dokumentacji API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });
});

// Rejestracja kontrolerów oraz repozytoriów
builder.Services.AddControllers();
builder.Services.AddScoped<IEmployeesRepository, EmployeesRepository>();
builder.Services.AddScoped<IShiftsRepository, ShiftsRepository>();
builder.Services.AddScoped<IReportsRepository, ReportsRepostiory>();
builder.Services.AddScoped<IRaportRepository, RaportRepository>();
builder.Services.AddScoped<IRolesRepository, RolesRepository>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Włącz CORS
app.UseCors("AllowFrontend");

// Autoryzacja JWT
app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();


app.MapControllers();

app.Run();
