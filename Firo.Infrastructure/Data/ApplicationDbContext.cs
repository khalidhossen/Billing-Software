using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Firo.Domain.Entities;

namespace Firo.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {

        }
        public DbSet<CompanyProfile> CompanyProfiles { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<LookUp> LookUps { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<PayDue> PayDues { get; set; }

    }
}
