using GroceryControl.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GroceryControl.Infrastructure.Persistence.Configurations;

public class PurchaseItemConfiguration : IEntityTypeConfiguration<PurchaseItem>
{
    public void Configure(EntityTypeBuilder<PurchaseItem> builder)
    {
        builder.ToTable("PurchaseItems");

        builder.HasKey(pi => pi.Id);

        builder.Property(pi => pi.Quantity)
            .HasPrecision(18, 4);

        builder.Property(pi => pi.UnitPrice)
            .HasPrecision(18, 2);

        builder.Property(pi => pi.TotalPrice)
            .HasPrecision(18, 2);

        builder.HasOne(pi => pi.Purchase)
            .WithMany(p => p.Items)
            .HasForeignKey(pi => pi.PurchaseId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(pi => pi.Product)
            .WithMany(p => p.PurchaseItems)
            .HasForeignKey(pi => pi.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(pi => pi.UnitType)
            .WithMany()
            .HasForeignKey(pi => pi.UnitTypeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
