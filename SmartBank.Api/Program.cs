using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

// SmartBank katmanları
using SmartBank.Infrastructure.Persistence;            // CustomerCoreDbContext
using SmartBank.Application.Interfaces;               // IClearingService
using SmartBank.Application.Services;                 // ClearingService
using SmartBank.Application.MappingProfiles;          // ClearingProfile

var builder = WebApplication.CreateBuilder(args);

// ---- DbContext ----
// appsettings.json içinde "ConnectionStrings:DefaultConnection" olmalı
builder.Services.AddDbContext<CustomerCoreDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ---- AutoMapper ----
builder.Services.AddAutoMapper(typeof(ClearingProfile).Assembly);

// ---- Uygulama Servisleri (DI) ----
builder.Services.AddScoped<IClearingService, ClearingService>();

// ---- Controllers + JSON ----
builder.Services.AddControllers()
    .AddJsonOptions(o =>
    {
        o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        o.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

// ---- Swagger ----
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "SmartBank API", Version = "v1" });
    c.MapType<IFormFile>(() => new OpenApiSchema { Type = "string", Format = "binary" });
    c.CustomSchemaIds(t => t.FullName);
    c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
});

var app = builder.Build();

// ---- Dev ortamında Swagger ----
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "SmartBank API v1");
        c.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

// Basit sağlık kontrolü (opsiyonel)
app.MapGet("/healthz", () => "ok");

app.Run();
