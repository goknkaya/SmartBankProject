using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using SmartBank.Application.DTOs.Validators.Card;
using SmartBank.Application.DTOs.Validators.Customer;
using SmartBank.Application.Interfaces;
using SmartBank.Application.Services;
using SmartBank.Infrastructure.Persistence;
using SmartBank.Application.MappingProfiles;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ✅ AutoMapper
builder.Services.AddAutoMapper(typeof(CardProfile));
builder.Services.AddAutoMapper(typeof(CustomerProfile));

// ✅ FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<CreateCustomerDtoValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<UpdateCustomerDtoValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<DeleteCustomerDtoValidator>();

builder.Services.AddValidatorsFromAssemblyContaining<CreateCardDtoValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<UpdateCardDtoValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<DeleteCardDtoValidator>();

// ✅ Dependency Injection
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<ICardService, CardService>();

// ✅ Veritabanı bağlantısı
builder.Services.AddDbContext<CustomerCoreDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();
