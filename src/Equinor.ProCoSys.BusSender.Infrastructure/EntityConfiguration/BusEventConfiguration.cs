﻿using Equinor.ProCoSys.BusSender.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Equinor.ProCoSys.BusSender.Infrastructure.EntityConfiguration
{
    public class BusEventConfiguration : IEntityTypeConfiguration<BusEvent>
    {
        public void Configure(EntityTypeBuilder<BusEvent> builder)
        {
            builder.ToTable("BUSEVENT");
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id).HasColumnName("ID");
            builder.Property(p => p.Created).HasColumnName("CREATED");
            builder.Property(p => p.Event).HasColumnName("EVENT");
            builder.Property(p => p.Message).HasColumnName("MESSAGE");
            builder.Property(p => p.Sent).HasColumnName("SENT");
        }
    }
}
