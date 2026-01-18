using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SmartBank.Application.DTOs.Validators.Customer;
using SmartBank.Application.DTOs.Validators.Reversal;
using SmartBank.Application.Interfaces;
using SmartBank.Application.MappingProfiles;
using SmartBank.Application.Services;
using SmartBank.Application.Validators.Switch;
using SmartBank.Infrastructure.Persistence;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// ================= DB =================
builder.Services.AddDbContext<CustomerCoreDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
       .EnableDetailedErrors()
       .EnableSensitiveDataLogging()); // teşhis için

// ============== AutoMapper & FluentValidation ==============
builder.Services.AddAutoMapper(typeof(CustomerProfile).Assembly);
builder.Services.AddAutoMapper(typeof(ClearingProfile).Assembly);
builder.Services.AddAutoMapper(typeof(ReversalProfile).Assembly);
builder.Services.AddAutoMapper(typeof(SwitchProfile).Assembly);

builder.Services.AddFluentValidationAutoValidation()
                .AddFluentValidationClientsideAdapters();

// Customer validator’larını KAYDET
builder.Services.AddValidatorsFromAssemblyContaining<CreateCustomerDtoValidator>();
builder.Services.AddValidatorsFromAssembly(typeof(ClearingProfile).Assembly);
builder.Services.AddValidatorsFromAssemblyContaining<CreateReversalDtoValidator>();
builder.Services.AddValidatorsFromAssembly(typeof(CreateSwitchMessageDtoValidator).Assembly);

// ================== DI ==================
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<ICardService, CardService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<IReversalService, ReversalService>();
builder.Services.AddScoped<IChargebackService, ChargebackService>();
builder.Services.AddScoped<IClearingService, ClearingService>();
builder.Services.AddScoped<ISwitchService, SwitchService>();
builder.Services.AddScoped<IFraudService, FraudService>();

// ============= Controllers + JSON =============
builder.Services.AddControllers().AddJsonOptions(o =>
{
    o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    o.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});

// ================== JWT ==================
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

// ================= Swagger =================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "SmartBank API", Version = "v1" });
    c.CustomSchemaIds(t => t.FullName);
    c.MapType<IFormFile>(() => new OpenApiSchema { Type = "string", Format = "binary" });

    // JWT auth button
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

// ======= Global Exception Handler: İÇ MESAJI DÖKSÜN (teşhis için) =======
app.UseExceptionHandler(config =>
{
    config.Run(async context =>
    {
        var ex = context.Features.Get<IExceptionHandlerFeature>()?.Error;
        var status = 500;
        var message = ex?.GetBaseException().Message ?? "Sunucu hatası.";

        if (ex is DbUpdateException) status = 409;
        else if (ex is FluentValidation.ValidationException) status = 400;
        else if (ex is InvalidOperationException) status = 409;

        context.Response.StatusCode = status;
        context.Response.ContentType = "text/plain; charset=utf-8";
        await context.Response.WriteAsync(message);
    });
});

// ============== Swagger (anonim) ==============
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

// ============== Anonim yardımcı uçlar ==============
app.MapGet("/", () => Results.Redirect("/swagger")).AllowAnonymous();
app.MapGet("/healthz", () => Results.Ok(new { status = "ok", time = DateTime.Now })).AllowAnonymous();

// ============== Tüm controller'lar JWT ister ==============
//app.MapControllers().RequireAuthorization();
app.MapControllers();

app.Run();
