using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SmartBank.Domain.Entities;

namespace SmartBank.Infrastructure.Persistence
{
    public class CustomerCoreDbContext : DbContext
    {
        public CustomerCoreDbContext(DbContextOptions<CustomerCoreDbContext> options) : base(options) { }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Card> Cards { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Reversal> Reversals { get; set; }

    }
}
