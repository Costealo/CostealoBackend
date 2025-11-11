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

// Si vas a usar el servicio RapidAPI (CostealoBackend.Services.UnitConversionService)
using CostealoBackend.Services.Interfaces;
using IUnitConversionService = Costealo.Services.Contracts.IUnitConversionService;
using RapidSvc = CostealoBackend.Services.UnitConversionService;

var builder = WebApplication.CreateBuilder(args);

// DB temporal en memoria (cámbialo a SQL Server cuando quieras)
builder.Services.AddDbContext<AppDbContext>(o => o.UseInMemoryDatabase("CostealoDb"));
// Ejemplo para SQL Server (cuando cambies):
// builder.Services.AddDbContext<AppDbContext>(o =>
//     o.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();
builder.Services.AddCors(o =>
    o.AddPolicy("AllowAll", p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

// Swagger + Bearer
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
        Description = "Escribe: Bearer {tu_token_jwt}",
        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
    };
    c.AddSecurityDefinition("Bearer", scheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement { { scheme, Array.Empty<string>() } });
});

// DI servicios internos
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IWorkbookService, WorkbookService>();
builder.Services.AddScoped<IUnitConversionService, UnitConversionService>(); // conversión local (kg↔g, l↔ml, etc.)
builder.Services.AddScoped<IBlobService, AzureBlobService>();
builder.Services.AddScoped<IExcelParser, ExcelParser>();

// HttpClient para RapidAPI Unit Conversion (cliente nombrado)
builder.Services.AddHttpClient("RapidApiUnit", client =>
{
    var cfg  = builder.Configuration.GetSection("RapidApi");
    var baseUrl = cfg["BaseUrl"];
    var host = cfg["Host"];
    var key  = cfg["Key"];

    if (!string.IsNullOrWhiteSpace(baseUrl))
        client.BaseAddress = new Uri(baseUrl);

    // RapidAPI acepta headers por defecto; también puedes agregarlos por-request si prefieres.
    if (!string.IsNullOrWhiteSpace(host))
        client.DefaultRequestHeaders.Add("x-rapidapi-host", host);
    if (!string.IsNullOrWhiteSpace(key))
        client.DefaultRequestHeaders.Add("x-rapidapi-key", key);
});

// Servicio que consume RapidAPI (si lo usas)
builder.Services.AddScoped<IUnitConversionService, UnitConversionService>();
builder.Services.AddScoped<CostealoBackend.Services.Interfaces.IUnitConversionService, RapidSvc>();

// JWT (Issuer/Audience desde configuración + clave Base64)
byte[] GetJwtKeyBytes()
{
    var raw = builder.Configuration["Jwt:Key"] ?? "dev-key-please-change";
    try
    {
        // Si es Base64 válido, úsalo
        return Convert.FromBase64String(raw);
    }
    catch
    {
        // Si no era Base64, úsalo como texto plano UTF-8
        return Encoding.UTF8.GetBytes(raw);
    }
}

var keyBytes = GetJwtKeyBytes();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
