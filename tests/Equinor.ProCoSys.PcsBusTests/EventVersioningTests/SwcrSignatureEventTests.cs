using System;
using System.Collections.Generic;
using System.Linq;
using Equinor.ProCoSys.PcsServiceBus.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.PcsServiceBusTests.EventVersioningTests;

[TestClass]
public class SwcrSignatureEventTests
{
    [TestMethod]
    public void ISwcrSignatureEventV1_InterfacePropertiesAndMethods_DoNotChange()
    {
        // Arrange
        var swcrSignatureEventInterfaceType = typeof(ISwcrSignatureEventV1);
        var expectedProperties = new Dictionary<string, Type>
        {
            { "Plant", typeof(string) },
            { "ProCoSysGuid", typeof(Guid) },
            { "SwcrSignatureId", typeof(long) },
            { "ProjectName", typeof(string) },
            { "SwcrNo", typeof(string) },
            { "SwcrGuid", typeof(Guid) },
            { "SignatureRoleCode", typeof(string) },
            { "SignatureRoleDescription", typeof(string) },
            { "Sequence", typeof(int) },
            { "SignedByAzureOid", typeof(Guid?) },
            { "FunctionalRoleCode", typeof(string) },
            { "FunctionalRoleDescription", typeof(string) },
            { "SignedDate", typeof(DateTime?) },
            {"StatusCode", typeof(string) },
            { "LastUpdated", typeof(DateTime) }
        };

        // Act
        Dictionary<string, Type> actualProperties = swcrSignatureEventInterfaceType.GetProperties()
            .ToDictionary(p => p.Name, p => p.PropertyType);

        // Assert
        CollectionAssert.AreEquivalent(expectedProperties.Keys, actualProperties.Keys);
        foreach (KeyValuePair<string, Type> expectedProperty in expectedProperties)
            Assert.AreEqual(expectedProperty.Value, actualProperties[expectedProperty.Key]);
    }
}