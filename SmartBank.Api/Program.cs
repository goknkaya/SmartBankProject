using System.Text.Json.Serialization;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

using SmartBank.Infrastructure.Persistence;          // CustomerCoreDbContext
using SmartBank.Application.Interfaces;             // IService interfaces
using SmartBank.Application.Services;               // Service implementations
using SmartBank.Application.MappingProfiles;        // AutoMapper profiles root (any)

var builder = WebApplication.CreateBuilder(args);

// ---------------- DbContext ----------------
builder.Services.AddDbContext<CustomerCoreDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ---------------- AutoMapper ----------------
// Assembly scan: MappingProfiles altındaki tüm Profile’lar otomatik yüklenecek
builder.Services.AddAutoMapper(typeof(ClearingProfile).Assembly);

// ---------------- FluentValidation (opsiyonel ama önerilir) ----------------
builder.Services
    .AddFluentValidationAutoValidation()
    .AddFluentValidationClientsideAdapters();
// Validators klasörünü tara (Application assembly)
builder.Services.AddValidatorsFromAssembly(typeof(ClearingProfile).Assembly);

// ---------------- DI: Application Services ----------------
// Core
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<ICardService, CardService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<IReversalService, ReversalService>();

// Clearing
builder.Services.AddScoped<IClearingService, ClearingService>();

// Switching
builder.Services.AddScoped<ISwitchService, SwitchService>();

// ---------------- Controllers + JSON ----------------
builder.Services.AddControllers()
    .AddJsonOptions(o =>
    {
        o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        o.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

// ---------------- Swagger ----------------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "SmartBank API", Version = "v1" });
    // File upload parametreleri için
    c.MapType<IFormFile>(() => new OpenApiSchema { Type = "string", Format = "binary" });
    // Tip adı çakışmalarını engelle
    c.CustomSchemaIds(t => t.FullName);
    c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
});

var app = builder.Build();

// ---------------- Pipeline ----------------
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

// Basit sağlık kontrolü
app.MapGet("/healthz", () => Results.Ok("ok"));

app.Run();
