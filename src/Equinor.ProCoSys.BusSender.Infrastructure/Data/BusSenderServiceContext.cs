using System;
using System.Threading.Tasks;
using Dapper;
using Equinor.ProCoSys.BusSenderWorker.Core.Interfaces;
using Equinor.ProCoSys.BusSenderWorker.Core.Models;
using Equinor.ProCoSys.BusSenderWorker.Infrastructure.EntityConfiguration;
using Equinor.ProCoSys.BusSenderWorker.Infrastructure.Handlers;
using Microsoft.EntityFrameworkCore;

namespace Equinor.ProCoSys.BusSenderWorker.Infrastructure.Data;

public class BusSenderServiceContext : DbContext, IUnitOfWork
{
    public BusSenderServiceContext(DbContextOptions<BusSenderServiceContext> options) : base(options)
    {
        SqlMapper.AddTypeHandler(typeof(Guid), GuidTypeHandler.Default);
        SqlMapper.AddTypeHandler(typeof(DateTime), DateTimeUtcHandler.Default);
        SqlMapper.AddTypeHandler(typeof(bool), BooleanHandler.Default);
        SqlMapper.AddTypeHandler(typeof(DateOnly), DateOnlyHandler.Default);
    }

    public Task<int> SaveChangesAsync() => base.SaveChangesAsync();

    public void ClearChangeTracker() => base.ChangeTracker.Clear();

    public virtual DbSet<BusEvent> BusEvents { get; set; } = null!;


    protected override void OnModelCreating(ModelBuilder modelBuilder) =>
        modelBuilder.ApplyConfiguration(new BusEventConfiguration());
}
