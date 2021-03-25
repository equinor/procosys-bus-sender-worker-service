using System.Threading.Tasks;
using Equinor.ProCoSys.BusSender.Infrastructure.EntityConfiguration;
using Equinor.ProCoSys.BusSenderWorker.Core.Interfaces;
using Equinor.ProCoSys.BusSenderWorker.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Equinor.ProCoSys.BusSender.Infrastructure.Data
{
    public class BusSenderServiceContext : DbContext, IUnitOfWork
    {
        public BusSenderServiceContext(DbContextOptions<BusSenderServiceContext> options) : base(options) { }

        public virtual DbSet<BusEvent> BusEvents { get; set; } = null!;
        public Task<int> SaveChangesAsync() => base.SaveChangesAsync();


        protected override void OnModelCreating(ModelBuilder modelBuilder) => modelBuilder.ApplyConfiguration(new BusEventConfiguration());
    }
}
