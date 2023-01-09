

using Core.Persistence.Repositories;
using Core.Security.Entities;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Contexts
{
    public class BaseDbContext : DbContext
    {
        protected IConfiguration Configuration { get; set; }
        public DbSet<AdditionalService> AdditionalServices { get; set; }   
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Car> Cars { get; set; }
        public  DbSet<CarFileImage> CarFileImages { get; set; }
        public DbSet<CarDamage> CarDamages { get; set; }
        public DbSet<Color> Colors { get; set; }
        public DbSet<CorporateCustomer> CorporateCustomers { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<EmailAuthenticator> EmailAuthenticators { get; set; }
        public DbSet<FindeksCreditRate> FindeksCreditRates { get; set; }
        public DbSet<Fuel> Fuel { get; set; }
        public DbSet<IndividualCustomer> IndividualCustomers { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<Model> Models { get; set; }
        public DbSet<Rental> Rentals { get; set; }
        public DbSet<RentalsAdditionalService> RentalsAdditionalServices { get; set; }
        public DbSet<RentalBranch> RentalBranches { get; set; }
        public DbSet<OperationClaim> OperationClaims { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserOperationClaim> UserOperationClaims { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Transmission> Transmissions { get; set; }
        public DbSet<OtpAuthenticator> OtpAuthenticators { get; set; }



        public BaseDbContext(DbContextOptions dbContextOptions, IConfiguration configuration) : base(dbContextOptions)
        {
            Configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //if (!optionsBuilder.IsConfigured)
            //    base.OnConfiguring(
            //        optionsBuilder.UseSqlServer(Configuration.GetConnectionString("SomeConnectionString")));
        }
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {

            IEnumerable<EntityEntry<Entity>> datas = ChangeTracker
                .Entries<Entity>().Where(e =>
                    e.State == EntityState.Added || e.State == EntityState.Modified);

            foreach (var data in datas)
            {
                _ = data.State switch
                {
                    EntityState.Added => data.Entity.CreatedDate = DateTime.UtcNow,
                    EntityState.Modified => data.Entity.UpdatedDate = DateTime.UtcNow
                };
            }
            return await base.SaveChangesAsync(cancellationToken);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<Brand>(a =>
        //    {
        //        a.ToTable("Brands").HasKey(k => k.Id);
        //        a.Property(p => p.Id).HasColumnName("Id");
        //        a.Property(p => p.Name).HasColumnName("Name");

        //    });
        //    Brand[] brandEntitySeeds = { new(1, "BMW"), new(2, "Mercedes") };
        //    modelBuilder.Entity<Brand>().HasData(brandEntitySeeds);


        //    modelBuilder.Entity<Model>(a =>
        //    {
        //        a.ToTable("Models").HasKey(k => k.Id);
        //        a.Property(p => p.Id).HasColumnName("Id");
        //        a.Property(p => p.BrandId).HasColumnName("BrandId");
        //        a.Property(p => p.Name).HasColumnName("Name");
        //        a.Property(p => p.DailyPrice).HasColumnName("DailyPrice");
        //        a.Property(p => p.ImageUrl).HasColumnName("ImageUrl");
        //        a.HasOne(p => p.Brand);

        //    });
        //    //Model[] modelEntitySeeds = {new(1,1,"Series 4", 1500,""), new(2, 1, "Series 3", 1200, "")
        //    //        , new(3, 2, "A180", 1000, "") };
        //    //modelBuilder.Entity<Model>().HasData(modelEntitySeeds);


        //}
    }
}
