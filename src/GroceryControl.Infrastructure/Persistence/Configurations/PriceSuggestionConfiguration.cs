using GroceryControl.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GroceryControl.Infrastructure.Persistence.Configurations;

public class PriceSuggestionConfiguration : IEntityTypeConfiguration<PriceSuggestion>
{
    public void Configure(EntityTypeBuilder<PriceSuggestion> builder)
    {
        builder.ToTable("PriceSuggestions");

        builder.HasKey(ps => ps.Id);

        builder.Property(ps => ps.Price)
            .HasPrecision(18, 2);

        builder.Property(ps => ps.Quantity)
            .HasPrecision(18, 4);

        builder.HasIndex(ps => new { ps.ProductId, ps.StoreId, ps.ObservedDateUtc });

        builder.HasOne(ps => ps.Product)
            .WithMany(p => p.PriceSuggestions)
            .HasForeignKey(ps => ps.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ps => ps.Store)
            .WithMany()
            .HasForeignKey(ps => ps.StoreId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(ps => ps.UnitType)
            .WithMany()
            .HasForeignKey(ps => ps.UnitTypeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
