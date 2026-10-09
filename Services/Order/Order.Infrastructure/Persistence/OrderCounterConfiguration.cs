using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Order.Domain.Entities;

namespace Order.Infrastructure.Persistence.Configurations;

public sealed class OrderCounterConfiguration
    : IEntityTypeConfiguration<OrderCounter>
{
    public void Configure(
        EntityTypeBuilder<OrderCounter> builder)
    {
        builder.ToTable("OrderCounters");

        builder.HasKey(c => c.Date);

        builder.Property(c => c.Date)
            .HasColumnType("date");

        builder.Property(c => c.LastNumber)
            .IsRequired();
    }
}