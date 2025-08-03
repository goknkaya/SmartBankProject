using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using SmartBank.Infrastructure.Persistence;

public class CustomerCoreDbContextFactory : IDesignTimeDbContextFactory<CustomerCoreDbContext>
{
    public CustomerCoreDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<CustomerCoreDbContext>();
        optionsBuilder.UseSqlServer("Server=localhost;Database=SmartBankDb;Trusted_Connection=True;TrustServerCertificate=True;Encrypt=False");

        return new CustomerCoreDbContext(optionsBuilder.Options);
    }
}
