using System;
using System.Collections.Generic;
using System.Linq;
using Equinor.ProCoSys.PcsServiceBus.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.PcsServiceBusTests.EventVersioningTests
{
    [TestClass]
    public class TaskEventTests
    {
        [TestMethod]
        public void ITaskEventV1_InterfacePropertiesAndMethods_DoNotChange()
        {
            // Arrange
            var taskEventInterfaceType = typeof(ITaskEventV1);
            var expectedProperties = new Dictionary<string, Type>
            {
                {"Plant", typeof(string)},
                {"ProCoSysGuid", typeof(Guid)},
                {"TaskParentProCoSysGuid", typeof(Guid?)},
                {"ProjectName", typeof(string)},
                {"DocumentId", typeof(int)},
                {"Title", typeof(string)},
                {"TaskId", typeof(string)},
                {"ElementContentGuid", typeof(Guid)},
                {"Description", typeof(string)},
                {"Comments", typeof(string)},
                {"LastUpdated", typeof(DateTime)},
                {"SignedAt", typeof(DateTime?)},
                {"SignedBy", typeof(Guid?)},
            };

            // Act
            var actualProperties = taskEventInterfaceType.GetProperties()
                .ToDictionary(p => p.Name, p => p.PropertyType);

            // Assert
            CollectionAssert.AreEquivalent(expectedProperties.Keys, actualProperties.Keys);
            foreach (var expectedProperty in expectedProperties)
            {
                Assert.AreEqual(expectedProperty.Value, actualProperties[expectedProperty.Key]);
            }
        }
    }
}