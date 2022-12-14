using Equinor.ProCoSys.BusSenderWorker.Core.Interfaces;
using Equinor.ProCoSys.BusSenderWorker.Core.Services;
using Equinor.ProCoSys.PcsServiceBus.Topics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Equinor.ProCoSys.BusSenderWorker.Core.Tests;

[TestClass]
public class BusEventServiceTests
{
    private BusEventService _dut;
    private Mock<ITagDetailsRepository> _tagDetailsRepositoryMock;
    private Mock<IBusSenderMessageRepository> _busSenderMessageRepositoryMock;

    [TestInitialize]
    public void Setup()
    {
        _tagDetailsRepositoryMock = new Mock<ITagDetailsRepository>();

        _busSenderMessageRepositoryMock = new Mock<IBusSenderMessageRepository>();
        _dut = new BusEventService(_tagDetailsRepositoryMock.Object,_busSenderMessageRepositoryMock.Object);
    }

    [TestMethod]
    public async Task HandleBusEvents_AddsTagDetailsToTag()
    {
        //Arrange
        const string tagDetailsString = "HubbaBubba";
        _tagDetailsRepositoryMock.Setup(t => t.GetDetailsStringByTagId(116866654))
            .ReturnsAsync(tagDetailsString);
        const string jsonMessage =
            $@"{{
    ""TagNo"": ""P-PL470003"",
    ""TagId"": ""116866654"",
    ""Description"": ""Power cable (110 V < U < 400 V AC)"",
    ""ProjectName"": ""L.UKDBB.001"",
    ""McPkgNo"": ""4701-E001"",
    ""McPkgGuid"": ""EAC31FAFA85901BCE0532410000A10D9"",
    ""CommPkgNo"": ""4701-P01"",
    ""CommPkgGuid"": ""EAC31FA6823301BCE0532410000A10D9"",
    ""AreaCode"": ""P129"",
    ""AreaDescription"": ""Hypochlorite Room"",
    ""DisciplineCode"": ""E"",
    ""DisciplineDescription"": ""Electrical"",
    ""RegisterCode"": ""CABLE"",
    ""Status"": ""PLANNED"",
    ""System"": ""47"",
    ""CallOffNo"": """",
    ""PurchaseOrderNo"": ""BE505A"",
    ""TagFunctionCode"": ""PL"",
    ""InstallationCode"": ""DBB"",
    ""IsVoided"": false,
    ""Plant"": ""PCS$DOGGER_BANK_B"",
    ""PlantName"": ""Dogger Bank B"",
    ""EngineeringCode"": ""A"",
    ""MountedOn"": """",
    ""ProCoSysGuid"": ""EDF5C7E95F2C2006E0532710000AD705"",
    ""LastUpdated"": ""2022-12-12 11:25:26"",
    ""ProjectNames"": [
        ""L.UKDBB.001"",
        ""DOGGER_BANK_B_PRO""
    ]
}}";



        //Act
        var result =await _dut.AttachTagDetails(jsonMessage);

        //Assert

        var deserializedTagTopic = JsonSerializer.Deserialize<TagTopic>(result);
        Assert.IsNotNull(deserializedTagTopic);
        Assert.AreEqual(tagDetailsString,deserializedTagTopic.TagDetails);
        Assert.IsNotNull(deserializedTagTopic.McPkgGuid);
        Assert.IsNotNull(deserializedTagTopic.CommPkgGuid);

    }
}