using GroceryControl.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GroceryControl.Infrastructure.Persistence.Configurations;

public class InventoryEntryConfiguration : IEntityTypeConfiguration<InventoryEntry>
{
    public void Configure(EntityTypeBuilder<InventoryEntry> builder)
    {
        builder.ToTable("InventoryEntries");

        builder.HasKey(ie => ie.Id);

        builder.Property(ie => ie.CurrentQuantity)
            .HasPrecision(18, 4);

        builder.Property(ie => ie.MinimumThreshold)
            .HasPrecision(18, 4);

        builder.HasIndex(ie => new { ie.UserId, ie.ProductId })
            .IsUnique();

        builder.HasOne(ie => ie.User)
            .WithMany(u => u.InventoryEntries)
            .HasForeignKey(ie => ie.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ie => ie.Product)
            .WithMany(p => p.InventoryEntries)
            .HasForeignKey(ie => ie.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(ie => ie.UnitType)
            .WithMany()
            .HasForeignKey(ie => ie.UnitTypeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
