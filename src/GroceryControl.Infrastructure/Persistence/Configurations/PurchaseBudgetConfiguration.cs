using GroceryControl.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GroceryControl.Infrastructure.Persistence.Configurations;

public class PurchaseBudgetConfiguration : IEntityTypeConfiguration<PurchaseBudget>
{
    public void Configure(EntityTypeBuilder<PurchaseBudget> builder)
    {
        builder.ToTable("PurchaseBudgets");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.Amount)
            .HasPrecision(18, 2);

        builder.Property(b => b.Period)
            .IsRequired()
            .HasMaxLength(20);

        builder.HasIndex(b => b.UserId);

        builder.HasOne(b => b.User)
            .WithMany()
            .HasForeignKey(b => b.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
