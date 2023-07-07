using System;
using Equinor.ProCoSys.BusSenderWorker.Infrastructure.Handlers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.BusSenderWorker.Infrastructure.Tests;

[TestClass]
public class HandlerTests
{
    private GuidTypeHandler _guidTypeHandler = new GuidTypeHandler();

    [TestMethod]
    public void GuidTypeHandler_HandlesGuidsAsExpected()
    {
        string oracleGuid = "ED187F3F22E2C84BE0532810000A69DD";

         var parsedGuid = _guidTypeHandler.Parse(oracleGuid);

         Assert.AreEqual("ed187f3f-22e2-c84b-e053-2810000a69dd", parsedGuid.ToString());

         var gg = Guid.Parse("ED187F3F22E2C84BE0532810000A69DD");
         
         Assert.AreEqual(parsedGuid.ToString(), gg.ToString());


    }

}