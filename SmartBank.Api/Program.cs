using Microsoft.EntityFrameworkCore;
using SmartBank.Application.Interfaces;
using SmartBank.Application.Services;
using SmartBank.Domain.Entities;
using SmartBank.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// ---------------------------------------------
// ✅ Add services to the container
// ---------------------------------------------
builder.Services.AddControllers();

// ✅ Swagger servisleri (sadece dev ortamında çalışacak)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ✅ Dependency Injection
builder.Services.AddScoped<ICustomerService, CustomerService>();

// ✅ Veritabanı bağlantısı
builder.Services.AddDbContext<SmartBankDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// ---------------------------------------------
// ✅ Swagger middleware (yalnızca Development'ta çalışır)
// ---------------------------------------------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// ---------------------------------------------
// ✅ Diğer middleware
// ---------------------------------------------
app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

// ---------------------------------------------
// ✅ Dummy data ekleme (scope ile)
// ---------------------------------------------
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<SmartBankDbContext>();

    // Eğer veritabanında hiç müşteri yoksa örnek veri ekle
    if (!dbContext.Customers.Any())
    {
        dbContext.Customers.AddRange(
            new Customer
            {
                FirstName = "Gokhan",
                LastName = "Kaya",
                Email = "gokhankaya@example.com",
                PhoneNumber = "5551234567",
                NationalId = "11111111111",
                CreatedAt = DateTime.Now
            },
            new Customer
            {
                FirstName = "Cagla",
                LastName = "Kiral",
                Email = "caglakiral@example.com",
                PhoneNumber = "5557654321",
                NationalId = "22222222222",
                CreatedAt = DateTime.Now
            }
        );

        dbContext.SaveChanges();
    }
}

// ---------------------------------------------
// ✅ Uygulamayı başlat
// ---------------------------------------------
app.Run();
