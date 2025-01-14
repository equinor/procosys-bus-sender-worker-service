using Equinor.ProCoSys.BusSenderWorker.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Equinor.ProCoSys.BusSenderWorker.Infrastructure.EntityConfiguration;

public class PlantConfiguration : IEntityTypeConfiguration<Plant>
{
    public void Configure(EntityTypeBuilder<Plant> builder)
    {
        builder.ToTable("PROJECTSCHEMA");
        builder.HasKey(p => p.ProjectSchema);
        builder.Property(p => p.ProjectSchema).HasColumnName("PROJECTSCHEMA");
        builder.Property(p => p.IsVoided).HasColumnName("ISVOIDED");
    }
}
