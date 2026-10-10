using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OtaASimulator.Domain;

namespace OtaASimulator.Infrastructure.Persistence.Configurations;

public sealed class RateCalendarConfiguration
    : IEntityTypeConfiguration<RateCalendar>
{
    public void Configure(EntityTypeBuilder<RateCalendar> builder)
    {
        builder.ToTable("rate_calendar", "public", table =>
        {
            table.HasCheckConstraint("ck_rate_calendar_1",
                "nightly_price > 0");
            table.HasCheckConstraint("ck_rate_calendar_2",
                "minimum_nights_override > 0");
            table.HasCheckConstraint("ck_rate_calendar_3",
                "maximum_nights_override > 0");
            table.HasCheckConstraint("ck_rate_calendar_4",
                "minimum_nights_override IS NULL OR maximum_nights_override IS NULL OR maximum_nights_override >= minimum_nights_override");
        });
        builder.HasKey(x => new { x.RatePlanId, x.StayDate });

        builder.Property(x => x.RatePlanId)
            .HasColumnName("rate_plan_id")
            .IsRequired()
            .ValueGeneratedNever();

        builder.Property(x => x.StayDate)
            .HasColumnName("stay_date")
            .HasColumnType("date")
            .IsRequired()
            .ValueGeneratedNever();

        builder.Property(x => x.NightlyPrice)
            .HasColumnName("nightly_price")
            .HasPrecision(12, 2)
            .IsRequired();

        builder.Property(x => x.Closed)
            .HasColumnName("closed")
            .IsRequired()
            .HasDefaultValue(false)
            .HasSentinel(false);

        builder.Property(x => x.MinimumNightsOverride)
            .HasColumnName("minimum_nights_override")
            .IsRequired(false);

        builder.Property(x => x.MaximumNightsOverride)
            .HasColumnName("maximum_nights_override")
            .IsRequired(false);

        builder.Property(x => x.ClosedToArrival)
            .HasColumnName("closed_to_arrival")
            .IsRequired()
            .HasDefaultValue(false)
            .HasSentinel(false);

        builder.Property(x => x.ClosedToDeparture)
            .HasColumnName("closed_to_departure")
            .IsRequired()
            .HasDefaultValue(false)
            .HasSentinel(false);

        builder.Property(x => x.UpdatedAt)
            .HasColumnName("updated_at")
            .HasColumnType("timestamp with time zone")
            .IsRequired()
            .HasDefaultValueSql("now()")
            .ValueGeneratedOnAddOrUpdate();
        builder.Property(x => x.UpdatedAt).Metadata
            .SetBeforeSaveBehavior(PropertySaveBehavior.Ignore);
        builder.Property(x => x.UpdatedAt).Metadata
            .SetAfterSaveBehavior(PropertySaveBehavior.Ignore);

        builder.HasOne(x => x.RatePlan)
            .WithMany()
            .HasForeignKey(x => x.RatePlanId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.StayDate).HasDatabaseName("ix_rate_calendar_date");
    }
}
