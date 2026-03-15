using GroceryControl.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GroceryControl.Infrastructure.Persistence.Configurations;

public class PurchaseConfiguration : IEntityTypeConfiguration<Purchase>
{
    public void Configure(EntityTypeBuilder<Purchase> builder)
    {
        builder.ToTable("Purchases");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.TotalAmount)
            .HasPrecision(18, 2);

        builder.Property(p => p.Notes)
            .HasMaxLength(1000);

        builder.HasIndex(p => new { p.UserId, p.PurchaseDateUtc });

        builder.HasOne(p => p.User)
            .WithMany(u => u.Purchases)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(p => p.Store)
            .WithMany(s => s.Purchases)
            .HasForeignKey(p => p.StoreId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
