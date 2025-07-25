﻿using MeterReadingsApi.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeterReadingsApi.Data.Configurations
{
    public class MeterReadingConfiguration : IEntityTypeConfiguration<MeterReading>
    {
        public void Configure(EntityTypeBuilder<MeterReading> builder)
        {
            // Table name
            builder.ToTable("MeterReading");

            // Primary key 
            builder.HasKey(x => x.Id);

            // Column properties
            builder.Property(x => x.MeterReadingDateTime)
                .IsRequired()
                .HasColumnType("datetime2");

            builder.Property(x => x.MeterReadValue)
                .IsRequired();

            // Relationships
            builder.HasOne(x => x.Account)
                .WithMany(x => x.MeterReadings)
                .HasForeignKey(x => x.AccountId)
                .IsRequired();

            // Indexes
            // Create unique constraint to prevent duplicate readings at datase level
            builder.HasIndex(x => new { x.AccountId, x.MeterReadingDateTime, x.MeterReadValue })
                .IsUnique();
        }
    }
}
