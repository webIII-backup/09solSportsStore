using SportsStore.Models.Domain;
using Microsoft.EntityFrameworkCore;
using SportsStore.Data.Mappers;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace SportsStore.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Category> Categories { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfiguration(new ProductConfiguration());
            builder.ApplyConfiguration(new CityConfiguration());
            builder.ApplyConfiguration(new CustomerConfiguration());
            builder.ApplyConfiguration(new OrderConfiguration());
            builder.ApplyConfiguration(new OrderLineConfiguration());
            builder.ApplyConfiguration(new CategoryConfiguration());
            builder.Ignore<Cart>();
            builder.Ignore<CartLine>();
        }
    }
}
