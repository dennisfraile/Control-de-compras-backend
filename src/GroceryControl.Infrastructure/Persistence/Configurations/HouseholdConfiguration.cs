using GroceryControl.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GroceryControl.Infrastructure.Persistence.Configurations;

public class HouseholdConfiguration : IEntityTypeConfiguration<Household>
{
    public void Configure(EntityTypeBuilder<Household> builder)
    {
        builder.ToTable("Households");

        builder.HasKey(h => h.Id);

        builder.Property(h => h.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.HasOne(h => h.Owner)
            .WithMany()
            .HasForeignKey(h => h.OwnerUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
