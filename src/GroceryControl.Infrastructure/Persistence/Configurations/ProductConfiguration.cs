using GroceryControl.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GroceryControl.Infrastructure.Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.Brand)
            .HasMaxLength(200);

        builder.Property(p => p.Barcode)
            .HasMaxLength(50);

        builder.Property(p => p.ImageUrl)
            .HasMaxLength(2048);

        builder.Property(p => p.DefaultQuantity)
            .HasPrecision(18, 4);

        builder.HasIndex(p => p.Barcode)
            .IsUnique()
            .HasFilter("\"Barcode\" IS NOT NULL");

        builder.HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.DefaultUnitType)
            .WithMany(ut => ut.Products)
            .HasForeignKey(p => p.DefaultUnitTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.CreatedByUser)
            .WithMany()
            .HasForeignKey(p => p.CreatedByUserId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
