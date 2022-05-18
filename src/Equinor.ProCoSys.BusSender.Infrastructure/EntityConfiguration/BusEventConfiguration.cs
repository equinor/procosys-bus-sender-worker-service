using Equinor.ProCoSys.BusSenderWorker.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Equinor.ProCoSys.BusSenderWorker.Infrastructure.EntityConfiguration;

public class BusEventConfiguration : IEntityTypeConfiguration<BusEvent>
{
    public void Configure(EntityTypeBuilder<BusEvent> builder)
    {
        builder.ToTable("BUSEVENT");
        builder.Property(p => p.Id).HasColumnName("BUSEVENT_ID");
        builder.Property(p => p.Created).HasColumnName("CREATED");
        builder.Property(p => p.Event).HasColumnName("EVENT");
        builder.Property(p => p.Message).HasColumnName("MESSAGE");
        builder.Property(p => p.Sent).HasColumnName("SENT");
    }
}
