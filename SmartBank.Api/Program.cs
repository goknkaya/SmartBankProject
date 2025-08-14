using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using SmartBank.Application.DTOs.Validators.Card;
using SmartBank.Application.DTOs.Validators.Customer;
using SmartBank.Application.DTOs.Validators.Transaction;
using SmartBank.Application.DTOs.Validators.Reversal;
using SmartBank.Application.Interfaces;
using SmartBank.Application.Services;
using SmartBank.Infrastructure.Persistence;
using SmartBank.Application.MappingProfiles;

var builder = WebApplication.CreateBuilder(args);

// Controllers + FluentValidation middleware
builder.Services.AddControllers();
builder.Services.AddFluentValidationAutoValidation();          // ⇦ otomatik server-side validation
builder.Services.AddFluentValidationClientsideAdapters();      // ⇦ opsiyonel (Swagger/UI tarafı için)

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// AutoMapper (tek satırda tüm profilleri assembly’den yükler)
builder.Services.AddAutoMapper(typeof(CardProfile).Assembly);

// FluentValidation: validator’ları tara
builder.Services.AddValidatorsFromAssemblyContaining<CreateCustomerDtoValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<UpdateCustomerDtoValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<DeleteCustomerDtoValidator>();

builder.Services.AddValidatorsFromAssemblyContaining<CreateCardDtoValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<UpdateCardDtoValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<DeleteCardDtoValidator>();

builder.Services.AddValidatorsFromAssemblyContaining<CreateTransactionDtoValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateReversalDtoValidator>();

// DI
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<ICardService, CardService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<IReversalService, ReversalService>();

// DbContext
builder.Services.AddDbContext<CustomerCoreDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
