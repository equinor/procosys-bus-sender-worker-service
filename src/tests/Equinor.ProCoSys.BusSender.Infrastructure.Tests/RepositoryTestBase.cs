using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.BusSender.Infrastructure.Tests
{
    [TestClass]
    public class RepositoryTestBase
    {
        protected ContextHelper ContextHelper;

        [TestInitialize]
        public void RepositorySetup() => ContextHelper = new ContextHelper();
    }
}
