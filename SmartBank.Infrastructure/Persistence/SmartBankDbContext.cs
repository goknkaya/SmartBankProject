using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SmartBank.Domain.Entities;

namespace SmartBank.Infrastructure.Persistence
{
    public class SmartBankDbContext : DbContext
    {
        public SmartBankDbContext(DbContextOptions<SmartBankDbContext> options) : base(options) { }

        public DbSet<Customer> Customers { get; set; }
    }
}
