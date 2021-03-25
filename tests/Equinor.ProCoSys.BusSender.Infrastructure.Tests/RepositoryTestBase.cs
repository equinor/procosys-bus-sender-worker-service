using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.BusSenderWorker.Infrastructure.Tests
{
    public abstract class RepositoryTestBase
    {
        protected ContextHelper ContextHelper;

        [TestInitialize]
        public void RepositorySetup() => ContextHelper = new ContextHelper();
    }
}
