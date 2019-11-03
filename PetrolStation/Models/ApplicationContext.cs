using System;
using System.IO;
using PetrolStation.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace PetrolStation.Models
{
    public partial class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options)
              : base(options)
        {

        }

        public ApplicationContext() { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {

                optionsBuilder.UseSqlServer(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProductList>()
                .HasKey(c => new { c.IdProduct, c.IdTransaction });
            modelBuilder.Entity<PumpTank>()
                .HasKey(e => new { e.IdGasPump, e.IdFuelTank });
            
        }

        public DbSet<Car> Car { get; set; }
        public DbSet<Client> Client { get; set; }
        public DbSet<Fuel> Fuel { get; set; }
        public DbSet<Fueling> Fueling { get; set; }
        public DbSet<FuelingList> FuelingList { get; set; }
        public DbSet<FuelTank> FuelTank { get; set; }
        public DbSet<GasPump> GasPump { get; set; }
        public DbSet<LoyalityCard> LoyalityCard { get; set; }
        public DbSet<Product> Product { get; set; }
        public DbSet<ProductList> ProductList { get; set; }
        public DbSet<PumpTank> PumpTank { get; set; }
        public DbSet<Transaction> Transaction { get; set; }
        public DbSet<TransactionInvoice> TransactionInvoice { get; set; }
    }
}