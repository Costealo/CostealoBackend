using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

using Costealo.Data;
using Costealo.Services.Contracts;
using Costealo.Services.Security;
using Costealo.Services.Workbooks;
using Costealo.Services.Conversions;
using Costealo.Services.Storage;
using Costealo.Services.Parsing;

var builder = WebApplication.CreateBuilder(args);

// DB temporal en memoria (cámbialo a SQL Server cuando quieras)
builder.Services.AddDbContext<AppDbContext>(o => o.UseInMemoryDatabase("CostealoDb"));

builder.Services.AddControllers();
builder.Services.AddCors(o => o.AddPolicy("AllowAll", p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

// Swagger clásico
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "costealo", Version = "v1" });
    var scheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
    };
    c.AddSecurityDefinition("Bearer", scheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement { { scheme, Array.Empty<string>() } });
});

// DI
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IWorkbookService, WorkbookService>();
builder.Services.AddScoped<IUnitConversionService, UnitConversionService>();
builder.Services.AddScoped<IBlobService, AzureBlobService>();
builder.Services.AddScoped<IExcelParser, ExcelParser>();

// JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? "dev-key-please-change"))
        };
    });
builder.Services.AddAuthorization();
// HttpClient para RapidAPI Unit Conversion
builder.Services.AddHttpClient("RapidApiUnit", client =>
{
    var cfg = builder.Configuration.GetSection("RapidApi");
    var host = cfg["Host"];
    var key  = cfg["Key"];

    client.DefaultRequestHeaders.Add("x-rapidapi-host", host);
    client.DefaultRequestHeaders.Add("x-rapidapi-key",  key);
});

// Service DI
builder.Services.AddScoped<CostealoBackend.Services.Interfaces.IUnitConversionService,
    CostealoBackend.Services.UnitConversionService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseCors("AllowAll");

app.MapControllers();

app.Run();
