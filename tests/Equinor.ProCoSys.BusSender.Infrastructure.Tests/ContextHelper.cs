using Equinor.ProCoSys.BusSender.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Equinor.ProCoSys.BusSender.Infrastructure.Tests
{
    public class ContextHelper
    {
        public ContextHelper()
        {
            DbOptions = new DbContextOptions<BusSenderServiceContext>();
            ContextMock = new Mock<BusSenderServiceContext>(DbOptions);
        }

        public DbContextOptions<BusSenderServiceContext> DbOptions { get; }
        public Mock<BusSenderServiceContext> ContextMock { get; }
    }
}
