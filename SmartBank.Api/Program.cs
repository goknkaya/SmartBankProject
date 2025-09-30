using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SmartBank.Application.DTOs.Validators.Reversal;
using SmartBank.Application.Interfaces;
using SmartBank.Application.MappingProfiles;
using SmartBank.Application.Services;
using SmartBank.Application.Validators.Switch;
using SmartBank.Infrastructure.Persistence;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// DB
builder.Services.AddDbContext<CustomerCoreDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// AutoMapper & FluentValidation
builder.Services.AddAutoMapper(typeof(ClearingProfile).Assembly);
builder.Services.AddFluentValidationAutoValidation()
                .AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssembly(typeof(ClearingProfile).Assembly);
builder.Services.AddValidatorsFromAssembly(typeof(CreateSwitchMessageDtoValidator).Assembly);
builder.Services.AddAutoMapper(typeof(ReversalProfile).Assembly);
builder.Services.AddValidatorsFromAssemblyContaining<CreateReversalDtoValidator>();

// DI
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<ICardService, CardService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<IReversalService, ReversalService>();
builder.Services.AddScoped<IChargebackService, ChargebackService>();
builder.Services.AddScoped<IClearingService, ClearingService>();
builder.Services.AddScoped<ISwitchService, SwitchService>();

// Controllers + JSON
builder.Services.AddControllers().AddJsonOptions(o =>
{
    o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    o.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});

// JWT
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];
var jwtKey = builder.Configuration["Jwt:Key"];

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey ?? string.Empty)),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "SmartBank API", Version = "v1" });
    c.CustomSchemaIds(t => t.FullName);
    c.MapType<IFormFile>(() => new OpenApiSchema { Type = "string", Format = "binary" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Bearer {token}"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } }, Array.Empty<string>() }
    });
});

var app = builder.Build();

// *** Dev exception page KAPALI olsun (yoksa stack trace döner). ***
// if (app.Environment.IsDevelopment()) app.UseDeveloperExceptionPage();

// === GLOBAL EXCEPTION HANDLER (sadece kısa, düz metin) ===
app.UseExceptionHandler(config =>
{
    config.Run(async context =>
    {
        var feature = context.Features.Get<IExceptionHandlerFeature>();
        var ex = feature?.Error;

        // Varsayılanlar
        var status = 500;
        var message = "Sunucu hatası.";

        if (ex != null)
        {
            // Tip bazlı sade eşleme (istersen genişlet)
            message = ex.Message;
            status = ex switch
            {
                FluentValidation.ValidationException => 400,
                InvalidOperationException => 409,      // çakışma/kural ihlali
                ApplicationException => 400,           // iş kuralı
                _ => 500
            };
        }

        context.Response.StatusCode = status;
        context.Response.ContentType = "text/plain; charset=utf-8";
        await context.Response.WriteAsync(message);
    });
});

// Swagger ANONİM
app.MapSwagger().AllowAnonymous();
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "SmartBank API v1");
    c.RoutePrefix = "swagger";
});

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// Anonim yardımcı uçlar
app.MapGet("/", () => Results.Redirect("/swagger")).AllowAnonymous();
app.MapGet("/healthz", () => Results.Ok(new { status = "ok", time = DateTime.Now })).AllowAnonymous();

// Tüm controller’lar JWT ister
app.MapControllers().RequireAuthorization();

app.Run();
