using GroceryControl.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GroceryControl.Infrastructure.Persistence.Configurations;

public class ShoppingTemplateItemConfiguration : IEntityTypeConfiguration<ShoppingTemplateItem>
{
    public void Configure(EntityTypeBuilder<ShoppingTemplateItem> builder)
    {
        builder.HasKey(i => i.Id);
        builder.Property(i => i.Quantity).HasPrecision(18, 4);

        builder.HasOne(i => i.Product)
            .WithMany()
            .HasForeignKey(i => i.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(i => i.UnitType)
            .WithMany()
            .HasForeignKey(i => i.UnitTypeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
