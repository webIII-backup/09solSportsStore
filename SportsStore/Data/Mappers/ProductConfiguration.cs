using Microsoft.EntityFrameworkCore;
using SportsStore.Models.Domain;

namespace SportsStore.Data.Mappers
{
    internal class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Product");
            builder.HasKey(t => t.ProductId);
            builder.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(100);
        }
    }
}
