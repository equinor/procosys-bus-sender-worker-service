using System;
using Equinor.ProCoSys.BusSenderWorker.Core.Interfaces;
using Equinor.ProCoSys.BusSenderWorker.Core.Services;
using Equinor.ProCoSys.PcsServiceBus.Topics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Text.Json;
using System.Threading.Tasks;
using Equinor.ProCoSys.BusSenderWorker.Core.Extensions;
using Equinor.ProCoSys.BusSenderWorker.Core.Models;
using Equinor.ProCoSys.PcsServiceBus.Queries;

namespace Equinor.ProCoSys.BusSenderWorker.Core.Tests;

[TestClass]
public class BusEventServiceTests
{
    private BusEventService _dut;
    private Mock<ITagDetailsRepository> _tagDetailsRepositoryMock;
    private Mock<IBusSenderMessageRepository> _busSenderMessageRepositoryMock;
    private Mock<IDapperRepository> _dapperRepositoryMock;

    [TestInitialize]
    public void Setup()
    {
        _tagDetailsRepositoryMock = new Mock<ITagDetailsRepository>();

        _busSenderMessageRepositoryMock = new Mock<IBusSenderMessageRepository>();
        _dapperRepositoryMock = new Mock<IDapperRepository>();
        _dut = new BusEventService(_tagDetailsRepositoryMock.Object,_busSenderMessageRepositoryMock.Object,_dapperRepositoryMock.Object);
    }
    
    [TestMethod]
    public async Task CreateActionMessage_ValidMessage_ReturnsSerializedActionEvent()
    {
        // Arrange
        const string message = "123";
        const long actionId = 123L;
        var queryString = ActionQuery.GetQuery(actionId);
        var actionEvent = new ActionEvent
        {
            Plant = "Plant1",
            ProCoSysGuid = Guid.NewGuid(),
            ElementContentGuid = Guid.NewGuid(),
            CommPkgNo = "CommPkg1",
            CommPkgGuid = Guid.NewGuid(),
            SwcrNo = "SWCR1",
            SwcrGuid = Guid.NewGuid(),
            DocumentNo = "Document1",
            Description = "Description1",
            DocumentGuid = Guid.NewGuid(),
            ActionNo = "Action1",
            Title = "Title1",
            Comments = "Comments1",
            Deadline = DateOnly.MinValue,
            CategoryCode = "Category1",
            CategoryGuid = Guid.NewGuid(),
            PriorityCode = "Priority1",
            PriorityGuid = Guid.NewGuid(),
            RequestedByOid = Guid.NewGuid(),
            ActionByOid = Guid.NewGuid(),
            ActionByRole = "Role1",
            ActionByRoleGuid = Guid.NewGuid(),
            ResponsibleOid = Guid.NewGuid(),
            ResponsibleRole = "Role2",
            ResponsibleRoleGuid = Guid.NewGuid(),
            LastUpdated = DateTime.UtcNow,
            SignedAt = DateTime.UtcNow,
            SignedBy = Guid.NewGuid()
        };

        _dapperRepositoryMock.Setup(repo => repo.QuerySingle<ActionEvent>(queryString, message)).ReturnsAsync(actionEvent);

        // Act
        var result = await _dut.CreateActionMessage(message);

        // Assert
        Assert.IsNotNull(result);
        var deserializedResult = JsonSerializer.Deserialize<ActionEvent>(result, DefaultSerializerHelper.SerializerOptions);

        // Check if the properties are equal
        Assert.AreEqual(actionEvent.Plant, deserializedResult.Plant);
        Assert.AreEqual(actionEvent.ProCoSysGuid, deserializedResult.ProCoSysGuid);
        Assert.AreEqual(actionEvent.ElementContentGuid, deserializedResult.ElementContentGuid);
        Assert.AreEqual(actionEvent.CommPkgNo, deserializedResult.CommPkgNo);
        Assert.AreEqual(actionEvent.CommPkgGuid, deserializedResult.CommPkgGuid);
        Assert.AreEqual(actionEvent.SwcrNo, deserializedResult.SwcrNo);
        Assert.AreEqual(actionEvent.SwcrGuid, deserializedResult.SwcrGuid);
        Assert.AreEqual(actionEvent.DocumentNo, deserializedResult.DocumentNo);
        Assert.AreEqual(actionEvent.Description, deserializedResult.Description);
        Assert.AreEqual(actionEvent.DocumentGuid, deserializedResult.DocumentGuid);
        Assert.AreEqual(actionEvent.ActionNo, deserializedResult.ActionNo);
        Assert.AreEqual(actionEvent.Title, deserializedResult.Title);
        Assert.AreEqual(actionEvent.Comments, deserializedResult.Comments);
        Assert.AreEqual(actionEvent.Deadline, deserializedResult.Deadline);
        Assert.AreEqual(actionEvent.CategoryCode, deserializedResult.CategoryCode);
        Assert.AreEqual(actionEvent.CategoryGuid, deserializedResult.CategoryGuid);
        Assert.AreEqual(actionEvent.PriorityCode, deserializedResult.PriorityCode);
        Assert.AreEqual(actionEvent.PriorityGuid, deserializedResult.PriorityGuid);
        Assert.AreEqual(actionEvent.RequestedByOid, deserializedResult.RequestedByOid);
        Assert.AreEqual(actionEvent.ActionByOid, deserializedResult.ActionByOid);
        Assert.AreEqual(actionEvent.ActionByRole, deserializedResult.ActionByRole);
        Assert.AreEqual(actionEvent.ActionByRoleGuid, deserializedResult.ActionByRoleGuid);
        Assert.AreEqual(actionEvent.ResponsibleOid, deserializedResult.ResponsibleOid);
        Assert.AreEqual(actionEvent.ResponsibleRole, deserializedResult.ResponsibleRole);
        Assert.AreEqual(actionEvent.ResponsibleRoleGuid, deserializedResult.ResponsibleRoleGuid);
        Assert.AreEqual(actionEvent.LastUpdated, deserializedResult.LastUpdated);
        Assert.AreEqual(actionEvent.SignedAt, deserializedResult.SignedAt);
        Assert.AreEqual(actionEvent.SignedBy, deserializedResult.SignedBy);
        Assert.AreEqual(actionEvent.EventType, deserializedResult.EventType);

        Assert.AreEqual(actionEvent.EventType, deserializedResult.EventType);
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