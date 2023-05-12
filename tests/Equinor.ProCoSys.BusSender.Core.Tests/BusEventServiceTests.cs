using System;
using System.Text.Json;
using System.Threading.Tasks;
using Dapper;
using Equinor.ProCoSys.BusSenderWorker.Core.Extensions;
using Equinor.ProCoSys.BusSenderWorker.Core.Interfaces;
using Equinor.ProCoSys.BusSenderWorker.Core.Models;
using Equinor.ProCoSys.BusSenderWorker.Core.Services;
using Equinor.ProCoSys.PcsServiceBus.Queries;
using Equinor.ProCoSys.PcsServiceBus.Topics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.BusSenderWorker.Core.Tests;

[TestClass]
public class BusEventServiceTests
{
    private Mock<IEventRepository> _dapperRepositoryMock;
    private IBusEventService _dut;
    private Mock<ITagDetailsRepository> _tagDetailsRepositoryMock;

    [TestMethod]
    public async Task CreateActionMessage_ValidMessage_ReturnsSerializedActionEvent()
    {
        // Arrange
        const string message = "123";
        const long actionId = 123L;
        var query = ActionQuery.GetQuery(actionId);
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

        _dapperRepositoryMock.Setup(repo => repo.QuerySingle<ActionEvent>(
                It.Is<(string queryString, DynamicParameters)>( qs => qs.queryString == query.queryString)
                ,message))
            .ReturnsAsync(actionEvent);

        // Act
        var result = await _dut.CreateActionMessage(message);

        // Assert
        Assert.IsNotNull(result);
        var deserializedResult =
            JsonSerializer.Deserialize<ActionEvent>(result, DefaultSerializerHelper.SerializerOptions);

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
    public async Task CreateCallOffMessage_ValidMessage_ReturnsSerializedCallOffEvent()
    {
        // Arrange
        const string message = "123";
        const long callOffId = 123L;
        var query = CallOffQuery.GetQuery(callOffId);
        var callOffEvent = new CallOffEvent
        {
            Plant = "Plant1",
            ProCoSysGuid = Guid.NewGuid(),
            CallOffId = 123,
            PackageId = 456,
            PurchaseOrderNo = "PO-12345",
            IsCompleted = false,
            UseMcScope = true,
            LastUpdated = DateTime.UtcNow,
            IsVoided = false,
            CreatedAt = DateTime.UtcNow,
            CallOffNo = "CO-12345",
            Description = "Description1",
            ResponsibleCode = "Resp1",
            ContractorCode = "Contr1",
            SupplierCode = "Supp1",
            EstimatedTagCount = 50,
            FATPlanned = DateOnly.MinValue,
            PackagePlannedDelivery = DateOnly.MinValue,
            PackageActualDelivery = DateOnly.MinValue,
            PackageClosed = DateOnly.MinValue,
            McDossierSent = DateOnly.MinValue,
            McDossierReceived = DateOnly.MinValue
        };

        _dapperRepositoryMock.Setup(repo => repo.QuerySingle<CallOffEvent>(
            It.Is<(string queryString, DynamicParameters)>( qs => qs.queryString == query.query),
                message))
            .ReturnsAsync(callOffEvent);

        // Act
        var result = await _dut.CreateCallOffMessage(message);

        // Assert
        Assert.IsNotNull(result);
        var deserializedResult =
            JsonSerializer.Deserialize<CallOffEvent>(result, DefaultSerializerHelper.SerializerOptions);

        // Check if the properties are equal
        Assert.AreEqual(callOffEvent.Plant, deserializedResult.Plant);
        Assert.AreEqual(callOffEvent.ProCoSysGuid, deserializedResult.ProCoSysGuid);
        Assert.AreEqual(callOffEvent.CallOffId, deserializedResult.CallOffId);
        Assert.AreEqual(callOffEvent.PackageId, deserializedResult.PackageId);
        Assert.AreEqual(callOffEvent.PurchaseOrderNo, deserializedResult.PurchaseOrderNo);
        Assert.AreEqual(callOffEvent.IsCompleted, deserializedResult.IsCompleted);
        Assert.AreEqual(callOffEvent.UseMcScope, deserializedResult.UseMcScope);
        Assert.AreEqual(callOffEvent.LastUpdated, deserializedResult.LastUpdated);
        Assert.AreEqual(callOffEvent.IsVoided, deserializedResult.IsVoided);
        Assert.AreEqual(callOffEvent.CreatedAt, deserializedResult.CreatedAt);
        Assert.AreEqual(callOffEvent.CallOffNo, deserializedResult.CallOffNo);
        Assert.AreEqual(callOffEvent.Description, deserializedResult.Description);
        Assert.AreEqual(callOffEvent.ResponsibleCode, deserializedResult.ResponsibleCode);
        Assert.AreEqual(callOffEvent.ContractorCode, deserializedResult.ContractorCode);
        Assert.AreEqual(callOffEvent.SupplierCode, deserializedResult.SupplierCode);
        Assert.AreEqual(callOffEvent.EstimatedTagCount, deserializedResult.EstimatedTagCount);
        Assert.AreEqual(callOffEvent.FATPlanned, deserializedResult.FATPlanned);
        Assert.AreEqual(callOffEvent.PackagePlannedDelivery, deserializedResult.PackagePlannedDelivery);
        Assert.AreEqual(callOffEvent.PackageActualDelivery, deserializedResult.PackageActualDelivery);
        Assert.AreEqual(callOffEvent.PackageClosed, deserializedResult.PackageClosed);
        Assert.AreEqual(callOffEvent.McDossierSent, deserializedResult.McDossierSent);
        Assert.AreEqual(callOffEvent.McDossierReceived, deserializedResult.McDossierReceived);
        Assert.AreEqual(callOffEvent.EventType, deserializedResult.EventType);
    }

    [TestMethod]
    public async Task CreateChecklistEvent_ValidMessage_ReturnsSerializedCheckListEvent()
    {
        // Arrange
        const string message = "123";
        const long checklistId = 123L;
        var queryString = ChecklistQuery.GetQuery(checklistId);
        var checklistEvent = new ChecklistEvent
        {
            Plant = "Plant1",
            ProCoSysGuid = Guid.NewGuid(),
            ProjectName = "Project1",
            TagNo = "Tag1",
            TagId = 123,
            TagGuid = Guid.NewGuid(),
            TagRegisterId = 456,
            ChecklistId = 789,
            TagCategory = "Category1",
            SheetNo = "Sheet1",
            SubSheetNo = "SubSheet1",
            FormularType = "Type1",
            FormularGroup = "Group1",
            FormPhaseCode = "Phase1",
            FormPhaseGuid = new Guid(),
            SystemModule = "Module1",
            FormularDiscipline = "Discipline1",
            Revision = "Revision1",
            PipingRevisionMcPkNo = "PRMP-123",
            PipingRevisionMcPkGuid = Guid.NewGuid(),
            ResponsibleCode = "Responsible1",
            ResponsibleGuid = new Guid(),
            StatusCode = "Status1",
            StatusGuid = new Guid(),
            UpdatedAt = DateTime.UtcNow,
            LastUpdated = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            SignedAt = DateTime.UtcNow,
            VerifiedAt = DateTime.UtcNow
        };

        _dapperRepositoryMock.Setup(repo => repo.QuerySingle<ChecklistEvent>(
            It.Is<(string queryString, DynamicParameters)>( qs => qs.queryString == queryString.query),
                message))
            .ReturnsAsync(checklistEvent);

        // Act
        var result = await _dut.CreateChecklistMessage(message);

        // Assert
        Assert.IsNotNull(result);
        var deserializedResult =
            JsonSerializer.Deserialize<ChecklistEvent>(result, DefaultSerializerHelper.SerializerOptions);

        // Check if the properties are equal
        Assert.AreEqual(checklistEvent.Plant, deserializedResult.Plant);
        Assert.AreEqual(checklistEvent.ProCoSysGuid, deserializedResult.ProCoSysGuid);
        Assert.AreEqual(checklistEvent.ProjectName, deserializedResult.ProjectName);
        Assert.AreEqual(checklistEvent.TagNo, deserializedResult.TagNo);
        Assert.AreEqual(checklistEvent.TagId, deserializedResult.TagId);
        Assert.AreEqual(checklistEvent.TagGuid, deserializedResult.TagGuid);
        Assert.AreEqual(checklistEvent.TagRegisterId, deserializedResult.TagRegisterId);
        Assert.AreEqual(checklistEvent.ChecklistId, deserializedResult.ChecklistId);
        Assert.AreEqual(checklistEvent.TagCategory, deserializedResult.TagCategory);
        Assert.AreEqual(checklistEvent.SheetNo, deserializedResult.SheetNo);
        Assert.AreEqual(checklistEvent.SubSheetNo, deserializedResult.SubSheetNo);
        Assert.AreEqual(checklistEvent.FormularType, deserializedResult.FormularType);
        Assert.AreEqual(checklistEvent.FormularGroup, deserializedResult.FormularGroup);
        Assert.AreEqual(checklistEvent.FormPhaseCode, deserializedResult.FormPhaseCode);
        Assert.AreEqual(checklistEvent.FormPhaseGuid, deserializedResult.FormPhaseGuid);
        Assert.AreEqual(checklistEvent.SystemModule, deserializedResult.SystemModule);
        Assert.AreEqual(checklistEvent.FormularDiscipline, deserializedResult.FormularDiscipline);
        Assert.AreEqual(checklistEvent.Revision, deserializedResult.Revision);
        Assert.AreEqual(checklistEvent.PipingRevisionMcPkNo, deserializedResult.PipingRevisionMcPkNo);
        Assert.AreEqual(checklistEvent.PipingRevisionMcPkGuid, deserializedResult.PipingRevisionMcPkGuid);
        Assert.AreEqual(checklistEvent.ResponsibleCode, deserializedResult.ResponsibleCode);
        Assert.AreEqual(checklistEvent.ResponsibleGuid, deserializedResult.ResponsibleGuid);
        Assert.AreEqual(checklistEvent.StatusCode, deserializedResult.StatusCode);
        Assert.AreEqual(checklistEvent.StatusGuid, deserializedResult.StatusGuid);
        Assert.AreEqual(checklistEvent.UpdatedAt, deserializedResult.UpdatedAt);
        Assert.AreEqual(checklistEvent.LastUpdated, deserializedResult.LastUpdated);
        Assert.AreEqual(checklistEvent.CreatedAt, deserializedResult.CreatedAt);
        Assert.AreEqual(checklistEvent.SignedAt, deserializedResult.SignedAt);
        Assert.AreEqual(checklistEvent.VerifiedAt, deserializedResult.VerifiedAt);
        Assert.AreEqual(checklistEvent.EventType, deserializedResult.EventType);
    }

    [TestMethod]
    public async Task CreateCommPkgEvent_ValidMessage_ReturnsSerializedCommPkgEvent()
    {
        // Arrange
        const string message = "123";
        const long commPkgId = 123L;
        var queryString = CommPkgQuery.GetQuery(commPkgId);
        var commPkgEvent = new CommPkgEvent
        {
            Plant = "Plant1",
            ProCoSysGuid = Guid.NewGuid(),
            PlantName = "PlantName1",
            ProjectName = "Project1",
            CommPkgNo = "CP-12345",
            CommPkgId = 123,
            Description = "Description1",
            CommPkgStatus = "Status1",
            IsVoided = false,
            LastUpdated = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            DescriptionOfWork = "Work1",
            Remark = "Remark1",
            ResponsibleCode = "Resp1",
            ResponsibleDescription = "RespDesc1",
            AreaCode = "Area1",
            AreaDescription = "AreaDesc1",
            Phase = "Phase1",
            CommissioningIdentifier = "CommId1",
            Demolition = false,
            Priority1 = "Prio1",
            Priority2 = "Prio2",
            Priority3 = "Prio3",
            Progress = "In Progress",
            DCCommPkgStatus = "DCStatus1"
        };

        _dapperRepositoryMock.Setup(repo => repo.QuerySingle<CommPkgEvent>(
                It.Is<(string queryString, DynamicParameters)>( qs => qs.queryString == queryString.query),
                message))
            .ReturnsAsync(commPkgEvent);

        // Act
        var result = await _dut.CreateCommPkgMessage(message);

        // Assert
        Assert.IsNotNull(result);
        var deserializedResult =
            JsonSerializer.Deserialize<CommPkgEvent>(result, DefaultSerializerHelper.SerializerOptions);

        // Check if the properties are equal
        Assert.AreEqual(commPkgEvent.Plant, deserializedResult.Plant);
        Assert.AreEqual(commPkgEvent.ProCoSysGuid, deserializedResult.ProCoSysGuid);
        Assert.AreEqual(commPkgEvent.PlantName, deserializedResult.PlantName);
        Assert.AreEqual(commPkgEvent.ProjectName, deserializedResult.ProjectName);
        Assert.AreEqual(commPkgEvent.CommPkgNo, deserializedResult.CommPkgNo);
        Assert.AreEqual(commPkgEvent.CommPkgId, deserializedResult.CommPkgId);
        Assert.AreEqual(commPkgEvent.Description, deserializedResult.Description);
        Assert.AreEqual(commPkgEvent.CommPkgStatus, deserializedResult.CommPkgStatus);
        Assert.AreEqual(commPkgEvent.IsVoided, deserializedResult.IsVoided);
        Assert.AreEqual(commPkgEvent.LastUpdated, deserializedResult.LastUpdated);
        Assert.AreEqual(commPkgEvent.CreatedAt, deserializedResult.CreatedAt);
        Assert.AreEqual(commPkgEvent.DescriptionOfWork, deserializedResult.DescriptionOfWork);
        Assert.AreEqual(commPkgEvent.Remark, deserializedResult.Remark);
        Assert.AreEqual(commPkgEvent.ResponsibleCode, deserializedResult.ResponsibleCode);
        Assert.AreEqual(commPkgEvent.ResponsibleDescription, deserializedResult.ResponsibleDescription);
        Assert.AreEqual(commPkgEvent.AreaCode, deserializedResult.AreaCode);
        Assert.AreEqual(commPkgEvent.AreaDescription, deserializedResult.AreaDescription);
        Assert.AreEqual(commPkgEvent.Phase, deserializedResult.Phase);
        Assert.AreEqual(commPkgEvent.CommissioningIdentifier, deserializedResult.CommissioningIdentifier);
        Assert.AreEqual(commPkgEvent.Demolition, deserializedResult.Demolition);
        Assert.AreEqual(commPkgEvent.Priority1, deserializedResult.Priority1);
        Assert.AreEqual(commPkgEvent.Priority2, deserializedResult.Priority2);
        Assert.AreEqual(commPkgEvent.Priority3, deserializedResult.Priority3);
        Assert.AreEqual(commPkgEvent.Progress, deserializedResult.Progress);
        Assert.AreEqual(commPkgEvent.DCCommPkgStatus, deserializedResult.DCCommPkgStatus);
        Assert.AreEqual(commPkgEvent.EventType, deserializedResult.EventType);
    }

    [TestMethod]
    public async Task CreateCommPkgMilestoneEvent_ValidMessage_ReturnsSerializedCommPkgMilestoneEvent()
    {
        // Arrange
        const string message = "5f643eeb-b114-4fc4-b884-ade3f6ea63ce";
        const string commPkgGuid = "5f643eeb-b114-4fc4-b884-ade3f6ea63ce";
        var query = CommPkgMilestoneQuery.GetQuery(commPkgGuid);

        var commPkgMilestoneEvent = new CommPkgMilestoneEvent
        {
            Plant = "Plant1",
            ProCoSysGuid = Guid.NewGuid(),
            PlantName = "PlantName1",
            ProjectName = "Project1",
            CommPkgGuid = Guid.NewGuid(),
            CommPkgNo = "CP-12345",
            Code = "Code1",
            ActualDate = DateTime.UtcNow,
            PlannedDate = DateOnly.FromDateTime(DateTime.UtcNow),
            ForecastDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7)),
            Remark = "Remark1",
            IsSent = true,
            IsAccepted = true,
            IsRejected = false,
            LastUpdated = DateTime.UtcNow
        };

        _dapperRepositoryMock.Setup(repo => repo.QuerySingle<CommPkgMilestoneEvent>(
                It.Is<(string queryString, DynamicParameters)>( qs => qs.queryString == query.queryString),
                message))
            .ReturnsAsync(commPkgMilestoneEvent);

        // Act
        var result = await _dut.CreateCommPkgMilestoneMessage(message);

        // Assert
        Assert.IsNotNull(result);
        var deserializedResult =
            JsonSerializer.Deserialize<CommPkgMilestoneEvent>(result, DefaultSerializerHelper.SerializerOptions);

        // Check if the properties are equal
        Assert.AreEqual(commPkgMilestoneEvent.Plant, deserializedResult.Plant);
        Assert.AreEqual(commPkgMilestoneEvent.ProCoSysGuid, deserializedResult.ProCoSysGuid);
        Assert.AreEqual(commPkgMilestoneEvent.PlantName, deserializedResult.PlantName);
        Assert.AreEqual(commPkgMilestoneEvent.ProjectName, deserializedResult.ProjectName);
        Assert.AreEqual(commPkgMilestoneEvent.CommPkgGuid, deserializedResult.CommPkgGuid);
        Assert.AreEqual(commPkgMilestoneEvent.CommPkgNo, deserializedResult.CommPkgNo);
        Assert.AreEqual(commPkgMilestoneEvent.Code, deserializedResult.Code);
        Assert.AreEqual(commPkgMilestoneEvent.ActualDate, deserializedResult.ActualDate);
        Assert.AreEqual(commPkgMilestoneEvent.PlannedDate, deserializedResult.PlannedDate);
        Assert.AreEqual(commPkgMilestoneEvent.ForecastDate, deserializedResult.ForecastDate);
        Assert.AreEqual(commPkgMilestoneEvent.Remark, deserializedResult.Remark);
        Assert.AreEqual(commPkgMilestoneEvent.IsSent, deserializedResult.IsSent);
        Assert.AreEqual(commPkgMilestoneEvent.IsAccepted, deserializedResult.IsAccepted);
        Assert.AreEqual(commPkgMilestoneEvent.IsRejected, deserializedResult.IsRejected);
        Assert.AreEqual(commPkgMilestoneEvent.LastUpdated, deserializedResult.LastUpdated);
        Assert.AreEqual(commPkgMilestoneEvent.EventType, deserializedResult.EventType);
    }

    [TestMethod]
    public async Task CreateCommPkgOperationMessage_ValidMessage_ReturnsSerializedCommPkgOperationEvent()
    {
        // Arrange
        const string message = "123";
        const long commPkgId = 123L;
        var query = CommPkgOperationQuery.GetQuery(commPkgId);
        var commPkgOperationEvent = new CommPkgOperationEvent
        {
            Plant = "Plant1",
            ProCoSysGuid = Guid.NewGuid(),
            ProjectName = "Project1",
            CommPkgNo = "CP-12345",
            CommPkgGuid = Guid.NewGuid(),
            InOperation = true,
            ReadyForProduction = true,
            MaintenanceProgram = true,
            YellowLine = false,
            BlueLine = true,
            YellowLineStatus = "Status1",
            BlueLineStatus = "Status2",
            TemporaryOperationEst = true,
            PmRoutine = false,
            CommissioningResp = true,
            ValveBlindingList = true,
            LastUpdated = DateTime.UtcNow
        };

        _dapperRepositoryMock.Setup(repo => repo.QuerySingle<CommPkgOperationEvent>(
                It.Is<(string queryString, DynamicParameters)>( qs => qs.queryString == query.queryString),
                message))
            .ReturnsAsync(commPkgOperationEvent);

        // Act
        var result = await _dut.CreateCommPkgOperationMessage(message);

        // Assert
        Assert.IsNotNull(result);
        var deserializedResult =
            JsonSerializer.Deserialize<CommPkgOperationEvent>(result, DefaultSerializerHelper.SerializerOptions);

        // Check if the properties are equal
        Assert.AreEqual(commPkgOperationEvent.Plant, deserializedResult.Plant);
        Assert.AreEqual(commPkgOperationEvent.ProCoSysGuid, deserializedResult.ProCoSysGuid);
        Assert.AreEqual(commPkgOperationEvent.ProjectName, deserializedResult.ProjectName);
        Assert.AreEqual(commPkgOperationEvent.CommPkgNo, deserializedResult.CommPkgNo);
        Assert.AreEqual(commPkgOperationEvent.CommPkgGuid, deserializedResult.CommPkgGuid);
        Assert.AreEqual(commPkgOperationEvent.InOperation, deserializedResult.InOperation);
        Assert.AreEqual(commPkgOperationEvent.ReadyForProduction, deserializedResult.ReadyForProduction);
        Assert.AreEqual(commPkgOperationEvent.MaintenanceProgram, deserializedResult.MaintenanceProgram);
        Assert.AreEqual(commPkgOperationEvent.YellowLine, deserializedResult.YellowLine);
        Assert.AreEqual(commPkgOperationEvent.BlueLine, deserializedResult.BlueLine);
        Assert.AreEqual(commPkgOperationEvent.YellowLineStatus, deserializedResult.YellowLineStatus);
        Assert.AreEqual(commPkgOperationEvent.BlueLineStatus, deserializedResult.BlueLineStatus);
        Assert.AreEqual(commPkgOperationEvent.TemporaryOperationEst, deserializedResult.TemporaryOperationEst);
        Assert.AreEqual(commPkgOperationEvent.PmRoutine, deserializedResult.PmRoutine);
        Assert.AreEqual(commPkgOperationEvent.CommissioningResp, deserializedResult.CommissioningResp);
        Assert.AreEqual(commPkgOperationEvent.ValveBlindingList, deserializedResult.ValveBlindingList);
        Assert.AreEqual(commPkgOperationEvent.LastUpdated, deserializedResult.LastUpdated);
        Assert.AreEqual(commPkgOperationEvent.EventType, deserializedResult.EventType);
    }

    [TestMethod]
    public async Task CreateCommPkgQueryMessage_ValidMessage_ReturnsSerializedCommPkgQueryEvent()
    {
        // Arrange
        const string message = "123,456";
        const long commPkgId = 123L;
        const long documentId = 456L;
        var query = CommPkgQueryQuery.GetQuery(commPkgId, documentId);
        var commPkgQueryEvent = new CommPkgQueryEvent
        {
            Plant = "Plant1",
            ProCoSysGuid = Guid.NewGuid(),
            ProjectName = "Project1",
            CommPkgId = 12345,
            CommPkgGuid = Guid.NewGuid(),
            CommPkgNo = "CP-12345",
            DocumentId = 67890,
            QueryNo = "Q-001",
            QueryGuid = Guid.NewGuid(),
            LastUpdated = DateTime.UtcNow
        };


        _dapperRepositoryMock.Setup(repo => repo.QuerySingle<CommPkgQueryEvent>(
                It.Is<(string, DynamicParameters)>( qs => qs.Item1 == query.queryString),
                message))
            .ReturnsAsync(commPkgQueryEvent);

        // Act
        var result = await _dut.CreateCommPkgQueryMessage(message);

        // Assert
        Assert.IsNotNull(result);
        var deserializedResult =
            JsonSerializer.Deserialize<CommPkgQueryEvent>(result, DefaultSerializerHelper.SerializerOptions);

        // Check if the properties are equal
        Assert.AreEqual(commPkgQueryEvent.Plant, deserializedResult.Plant);
        Assert.AreEqual(commPkgQueryEvent.ProCoSysGuid, deserializedResult.ProCoSysGuid);
        Assert.AreEqual(commPkgQueryEvent.ProjectName, deserializedResult.ProjectName);
        Assert.AreEqual(commPkgQueryEvent.CommPkgId, deserializedResult.CommPkgId);
        Assert.AreEqual(commPkgQueryEvent.CommPkgGuid, deserializedResult.CommPkgGuid);
        Assert.AreEqual(commPkgQueryEvent.CommPkgNo, deserializedResult.CommPkgNo);
        Assert.AreEqual(commPkgQueryEvent.DocumentId, deserializedResult.DocumentId);
        Assert.AreEqual(commPkgQueryEvent.QueryNo, deserializedResult.QueryNo);
        Assert.AreEqual(commPkgQueryEvent.QueryGuid, deserializedResult.QueryGuid);
        Assert.AreEqual(commPkgQueryEvent.LastUpdated, deserializedResult.LastUpdated);
        Assert.AreEqual(commPkgQueryEvent.EventType, deserializedResult.EventType);
    }

    [TestMethod]
    public async Task CreateCommPkgTaskEvent_ValidMessage_ReturnsSerializedCommPkgTaskEvent()
    {
        // Arrange
        const string message = "123,456";
        const long commPkgId = 123L;
        const long taskId = 456L;
        var query = CommPkgTaskQuery.GetQuery(commPkgId, taskId);

        var commPkgTaskEvent = new CommPkgTaskEvent
        {
            Plant = "Plant1",
            ProjectName = "Project1",
            ProCoSysGuid = Guid.NewGuid(),
            TaskGuid = Guid.NewGuid(),
            CommPkgGuid = Guid.NewGuid(),
            CommPkgNo = "CP-12345",
            LastUpdated = DateTime.UtcNow
        };


        _dapperRepositoryMock.Setup(repo => repo.QuerySingle<CommPkgTaskEvent>(
                It.Is<(string, DynamicParameters)>( qs => qs.Item1 == query.queryString),
                message))
            .ReturnsAsync(commPkgTaskEvent);

        // Act
        var result = await _dut.CreateCommPkgTaskMessage(message);

        // Assert
        Assert.IsNotNull(result);
        var deserializedResult =
            JsonSerializer.Deserialize<CommPkgTaskEvent>(result, DefaultSerializerHelper.SerializerOptions);

        // Check if the properties are equal
        Assert.AreEqual(commPkgTaskEvent.Plant, deserializedResult.Plant);
        Assert.AreEqual(commPkgTaskEvent.ProjectName, deserializedResult.ProjectName);
        Assert.AreEqual(commPkgTaskEvent.ProCoSysGuid, deserializedResult.ProCoSysGuid);
        Assert.AreEqual(commPkgTaskEvent.TaskGuid, deserializedResult.TaskGuid);
        Assert.AreEqual(commPkgTaskEvent.CommPkgGuid, deserializedResult.CommPkgGuid);
        Assert.AreEqual(commPkgTaskEvent.CommPkgNo, deserializedResult.CommPkgNo);
        Assert.AreEqual(commPkgTaskEvent.LastUpdated, deserializedResult.LastUpdated);
        Assert.AreEqual(commPkgTaskEvent.EventType, deserializedResult.EventType);
    }

    [TestMethod]
    public async Task CreateHeatTraceMessage_ValidMessage_ReturnsSerializedHeatTraceEvent()
    {
        // Arrange
        const string message = "123";
        const long heatTraceId = 123L;
        var query = HeatTraceQuery.GetQuery(heatTraceId);

        var heatTraceEvent = new HeatTraceEvent
        {
            Plant = "Plant1",
            ProCoSysGuid = Guid.NewGuid(),
            HeatTraceId = 12345,
            CableId = 67890,
            CableGuid = Guid.NewGuid(),
            CableNo = "C-001",
            TagId = 98765,
            TagGuid = Guid.NewGuid(),
            TagNo = "T-001",
            SpoolNo = "SP-001",
            LastUpdated = DateTime.UtcNow
        };


        _dapperRepositoryMock.Setup(repo => repo.QuerySingle<HeatTraceEvent>(
                It.Is<(string, DynamicParameters)>( qs => qs.Item1 == query.queryString), 
                message))
            .ReturnsAsync(heatTraceEvent);

        // Act
        var result = await _dut.CreateHeatTraceMessage(message);

        // Assert
        Assert.IsNotNull(result);
        var deserializedResult =
            JsonSerializer.Deserialize<HeatTraceEvent>(result, DefaultSerializerHelper.SerializerOptions);

        // Check if the properties are equal
        Assert.AreEqual(heatTraceEvent.Plant, deserializedResult.Plant);
        Assert.AreEqual(heatTraceEvent.ProCoSysGuid, deserializedResult.ProCoSysGuid);
        Assert.AreEqual(heatTraceEvent.HeatTraceId, deserializedResult.HeatTraceId);
        Assert.AreEqual(heatTraceEvent.CableId, deserializedResult.CableId);
        Assert.AreEqual(heatTraceEvent.CableGuid, deserializedResult.CableGuid);
        Assert.AreEqual(heatTraceEvent.CableNo, deserializedResult.CableNo);
        Assert.AreEqual(heatTraceEvent.TagId, deserializedResult.TagId);
        Assert.AreEqual(heatTraceEvent.TagGuid, deserializedResult.TagGuid);
        Assert.AreEqual(heatTraceEvent.TagNo, deserializedResult.TagNo);
        Assert.AreEqual(heatTraceEvent.SpoolNo, deserializedResult.SpoolNo);
        Assert.AreEqual(heatTraceEvent.LastUpdated, deserializedResult.LastUpdated);
        Assert.AreEqual(heatTraceEvent.EventType, deserializedResult.EventType);
    }

    [TestMethod]
    public async Task CreateLibraryFieldMessage_ValidMessage_ReturnsSerializedLibraryFieldEvent()
    {
        // Arrange
        const string message = "5f643eeb-b114-4fc4-b884-ade3f6ea63ce";
        const string libraryFieldGuid = "5f643eeb-b114-4fc4-b884-ade3f6ea63ce";
        var query = LibraryFieldQuery.GetQuery(libraryFieldGuid);
        var libraryFieldEvent = new LibraryFieldEvent
        {
            Plant = "Plant1",
            ProCoSysGuid = Guid.NewGuid(),
            LibraryGuid = Guid.NewGuid(),
            LibraryType = "LibraryType1",
            Code = "Code1",
            ColumnName = "ColumnName1",
            ColumnType = "ColumnType1",
            StringValue = "StringValue1",
            DateValue = DateOnly.FromDateTime(DateTime.UtcNow),
            NumberValue = 12345.67,
            LibraryValueGuid = Guid.NewGuid(),
            LastUpdated = DateTime.UtcNow
        };

        _dapperRepositoryMock.Setup(repo => repo.QuerySingle<LibraryFieldEvent>(
            It.Is<(string, DynamicParameters)>( qs => qs.Item1 == query.queryString),
            message))
            .ReturnsAsync(libraryFieldEvent);

        // Act
        var result = await _dut.CreateLibraryFieldMessage(message);

        // Assert
        Assert.IsNotNull(result);
        var deserializedResult =
            JsonSerializer.Deserialize<LibraryFieldEvent>(result, DefaultSerializerHelper.SerializerOptions);

        // Check if the properties are equal
        Assert.AreEqual(libraryFieldEvent.Plant, deserializedResult.Plant);
        Assert.AreEqual(libraryFieldEvent.ProCoSysGuid, deserializedResult.ProCoSysGuid);
        Assert.AreEqual(libraryFieldEvent.LibraryGuid, deserializedResult.LibraryGuid);
        Assert.AreEqual(libraryFieldEvent.LibraryType, deserializedResult.LibraryType);
        Assert.AreEqual(libraryFieldEvent.Code, deserializedResult.Code);
        Assert.AreEqual(libraryFieldEvent.ColumnName, deserializedResult.ColumnName);
        Assert.AreEqual(libraryFieldEvent.ColumnType, deserializedResult.ColumnType);
        Assert.AreEqual(libraryFieldEvent.StringValue, deserializedResult.StringValue);
        Assert.AreEqual(libraryFieldEvent.DateValue, deserializedResult.DateValue);
        Assert.AreEqual(libraryFieldEvent.NumberValue, deserializedResult.NumberValue);
        Assert.AreEqual(libraryFieldEvent.LibraryValueGuid, deserializedResult.LibraryValueGuid);
        Assert.AreEqual(libraryFieldEvent.LastUpdated, deserializedResult.LastUpdated);
        Assert.AreEqual(libraryFieldEvent.EventType, deserializedResult.EventType);
    }

    [TestMethod]
    public async Task CreateLibraryMessage_ValidMessage_ReturnsSerializedLibraryEvent()
    {
        //Arrange
        const string message = "123";
        const long libraryId = 123L;
        var query = LibraryQuery.GetQuery(libraryId);


        var libraryEvent = new LibraryEvent
        {
            Plant = "Plant1",
            ProCoSysGuid = Guid.NewGuid(),
            LibraryId = 1,
            ParentId = 2,
            ParentGuid = Guid.NewGuid(),
            Code = "Code1",
            Description = "Description1",
            IsVoided = false,
            Type = "Type1",
            LastUpdated = DateTime.UtcNow
        };

        _dapperRepositoryMock.Setup(repo => repo.QuerySingle<LibraryEvent>(
                It.Is<(string, DynamicParameters)>( qs => qs.Item1 == query.query),
                 message))
            .ReturnsAsync(libraryEvent);

        // Act
        var result = await _dut.CreateLibraryMessage(message);

        // Assert
        Assert.IsNotNull(result);
        var deserializedResult =
            JsonSerializer.Deserialize<LibraryEvent>(result, DefaultSerializerHelper.SerializerOptions);

        // Check if the properties are equal
        Assert.AreEqual(libraryEvent.Plant, deserializedResult.Plant);
        Assert.AreEqual(libraryEvent.ProCoSysGuid, deserializedResult.ProCoSysGuid);
        Assert.AreEqual(libraryEvent.LibraryId, deserializedResult.LibraryId);
        Assert.AreEqual(libraryEvent.ParentId, deserializedResult.ParentId);
        Assert.AreEqual(libraryEvent.ParentGuid, deserializedResult.ParentGuid);
        Assert.AreEqual(libraryEvent.Code, deserializedResult.Code);
        Assert.AreEqual(libraryEvent.Description, deserializedResult.Description);
        Assert.AreEqual(libraryEvent.IsVoided, deserializedResult.IsVoided);
        Assert.AreEqual(libraryEvent.Type, deserializedResult.Type);
        Assert.AreEqual(libraryEvent.LastUpdated, deserializedResult.LastUpdated);
        Assert.AreEqual(libraryEvent.EventType, deserializedResult.EventType);
    }

    [TestMethod]
    public async Task CreateLoopContentMessage_ValidMessage_ReturnsSerializedLoopContentEvent()
    {
        // Arrange
        const string message = "123";
        const long loopId = 123L;
        var query = LoopContentQuery.GetQuery(loopId);

        // Arrange
        var loopContentEvent = new LoopContentEvent
        {
            Plant = "TestPlant",
            ProCoSysGuid = Guid.NewGuid(),
            LoopTagId = 1234,
            LoopTagGuid = Guid.NewGuid(),
            TagId = 5678,
            TagGuid = Guid.NewGuid(),
            RegisterCode = "A123",
            LastUpdated = DateTime.UtcNow
        };

        _dapperRepositoryMock.Setup(repo => repo.QuerySingle<LoopContentEvent>(
                It.Is<(string, DynamicParameters)>(qs => qs.Item1 == query.queryString),
                message))
            .ReturnsAsync(loopContentEvent);

        // Act
        var result = await _dut.CreateLoopContentMessage(message);

        // Assert
        Assert.IsNotNull(result);
        var deserializedResult =
            JsonSerializer.Deserialize<LoopContentEvent>(result, DefaultSerializerHelper.SerializerOptions);

        // Check if the properties are equal
        Assert.IsNotNull(loopContentEvent);
        Assert.AreEqual(loopContentEvent.Plant, deserializedResult.Plant);
        Assert.AreEqual(loopContentEvent.ProCoSysGuid, deserializedResult.ProCoSysGuid);
        Assert.AreEqual(loopContentEvent.LoopTagId, deserializedResult.LoopTagId);
        Assert.AreEqual(loopContentEvent.LoopTagGuid, deserializedResult.LoopTagGuid);
        Assert.AreEqual(loopContentEvent.TagId, deserializedResult.TagId);
        Assert.AreEqual(loopContentEvent.TagGuid, deserializedResult.TagGuid);
        Assert.AreEqual(loopContentEvent.RegisterCode, deserializedResult.RegisterCode);
        Assert.AreEqual(loopContentEvent.LastUpdated, deserializedResult.LastUpdated);
    }

    [TestMethod]
    public async Task CreateMcPkgMessage_ValidMessage_ReturnsSerializedMcPkgEvent()
    {
        // Arrange
        const string message = "123";
        const long mcPkgId = 123L;
        var query = McPkgQuery.GetQuery(mcPkgId);

        var mcPkgEvent = new McPkgEvent
        {
            Plant = "Plant1",
            ProCoSysGuid = Guid.NewGuid(),
            PlantName = "PlantName1",
            ProjectName = "Project1",
            McPkgNo = "McPkg1",
            McPkgId = 1,
            CommPkgNo = "CommPkg1",
            CommPkgGuid = Guid.NewGuid(),
            Description = "Description1",
            Remark = "Remark1",
            ResponsibleCode = "RCode1",
            ResponsibleDescription = "ResponsibleDesc1",
            AreaCode = "Area1",
            AreaDescription = "AreaDesc1",
            Discipline = "Discipline1",
            McStatus = "Status1",
            Phase = "Phase1",
            IsVoided = false,
            CreatedAt = DateTime.UtcNow,
            LastUpdated = DateTime.UtcNow
        };

        _dapperRepositoryMock.Setup(repo => repo.QuerySingle<McPkgEvent>(
            It.Is<(string, DynamicParameters)>( qs => qs.Item1 == query.queryString),
            message))
            .ReturnsAsync(mcPkgEvent);

        // Act
        var result = await _dut.CreateMcPkgMessage(message);

        // Assert
        Assert.IsNotNull(result);
        var deserializedResult =
            JsonSerializer.Deserialize<McPkgEvent>(result, DefaultSerializerHelper.SerializerOptions);

        // Check if the properties are equal
        Assert.AreEqual(mcPkgEvent.Plant, deserializedResult.Plant);
        Assert.AreEqual(mcPkgEvent.ProCoSysGuid, deserializedResult.ProCoSysGuid);
        Assert.AreEqual(mcPkgEvent.PlantName, deserializedResult.PlantName);
        Assert.AreEqual(mcPkgEvent.ProjectName, deserializedResult.ProjectName);
        Assert.AreEqual(mcPkgEvent.McPkgNo, deserializedResult.McPkgNo);
        Assert.AreEqual(mcPkgEvent.McPkgId, deserializedResult.McPkgId);
        Assert.AreEqual(mcPkgEvent.CommPkgNo, deserializedResult.CommPkgNo);
        Assert.AreEqual(mcPkgEvent.CommPkgGuid, deserializedResult.CommPkgGuid);
        Assert.AreEqual(mcPkgEvent.Description, deserializedResult.Description);
        Assert.AreEqual(mcPkgEvent.Remark, deserializedResult.Remark);
        Assert.AreEqual(mcPkgEvent.ResponsibleCode, deserializedResult.ResponsibleCode);
        Assert.AreEqual(mcPkgEvent.ResponsibleDescription, deserializedResult.ResponsibleDescription);
        Assert.AreEqual(mcPkgEvent.AreaCode, deserializedResult.AreaCode);
        Assert.AreEqual(mcPkgEvent.AreaDescription, deserializedResult.AreaDescription);
        Assert.AreEqual(mcPkgEvent.Discipline, deserializedResult.Discipline);
        Assert.AreEqual(mcPkgEvent.McStatus, deserializedResult.McStatus);
        Assert.AreEqual(mcPkgEvent.Phase, deserializedResult.Phase);
        Assert.AreEqual(mcPkgEvent.IsVoided, deserializedResult.IsVoided);
        Assert.AreEqual(mcPkgEvent.CreatedAt, deserializedResult.CreatedAt);
        Assert.AreEqual(mcPkgEvent.LastUpdated, deserializedResult.LastUpdated);
        Assert.AreEqual(mcPkgEvent.EventType, deserializedResult.EventType);
    }

    [TestMethod]
    public async Task CreateMcPkgMilestoneMessage_ValidMessage_ReturnsSerializedMcPkgMilestoneEvent()
    {
        // Arrange
        const string message = "5f643eeb-b114-4fc4-b884-ade3f6ea63ce";
        
        const string mcPkgGuid = "5f643eeb-b114-4fc4-b884-ade3f6ea63ce";
        
        var query = McPkgMilestoneQuery.GetQuery(mcPkgGuid);

        var mcPkgMilestoneEvent = new McPkgMilestoneEvent
        {
            Plant = "Plant1",
            ProCoSysGuid = Guid.NewGuid(),
            PlantName = "PlantName1",
            ProjectName = "ProjectName1",
            McPkgGuid = Guid.NewGuid(),
            McPkgNo = "McPkgNo1",
            Code = "Code1",
            ActualDate = DateTime.UtcNow,
            PlannedDate = DateOnly.FromDateTime(DateTime.UtcNow),
            ForecastDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5)),
            Remark = "Remark1",
            IsSent = true,
            IsAccepted = true,
            IsRejected = false,
            LastUpdated = DateTime.UtcNow
        };

        _dapperRepositoryMock.Setup(repo => repo.QuerySingle<McPkgMilestoneEvent>(
                It.Is<(string, DynamicParameters)>( qs => qs.Item1 == query.queryString),
                message))
            .ReturnsAsync(mcPkgMilestoneEvent);

        // Act
        var result = await _dut.CreateMcPkgMilestoneMessage(message);

        // Assert
        Assert.IsNotNull(result);
        var deserializedResult =
            JsonSerializer.Deserialize<McPkgMilestoneEvent>(result, DefaultSerializerHelper.SerializerOptions);

        // Check if the properties are equal
        Assert.AreEqual(mcPkgMilestoneEvent.Plant, deserializedResult.Plant);
        Assert.AreEqual(mcPkgMilestoneEvent.ProCoSysGuid, deserializedResult.ProCoSysGuid);
        Assert.AreEqual(mcPkgMilestoneEvent.PlantName, deserializedResult.PlantName);
        Assert.AreEqual(mcPkgMilestoneEvent.ProjectName, deserializedResult.ProjectName);
        Assert.AreEqual(mcPkgMilestoneEvent.McPkgGuid, deserializedResult.McPkgGuid);
        Assert.AreEqual(mcPkgMilestoneEvent.McPkgNo, deserializedResult.McPkgNo);
        Assert.AreEqual(mcPkgMilestoneEvent.Code, deserializedResult.Code);
        Assert.AreEqual(mcPkgMilestoneEvent.ActualDate, deserializedResult.ActualDate);
        Assert.AreEqual(mcPkgMilestoneEvent.PlannedDate, deserializedResult.PlannedDate);
        Assert.AreEqual(mcPkgMilestoneEvent.ForecastDate, deserializedResult.ForecastDate);
        Assert.AreEqual(mcPkgMilestoneEvent.Remark, deserializedResult.Remark);
        Assert.AreEqual(mcPkgMilestoneEvent.IsSent, deserializedResult.IsSent);
        Assert.AreEqual(mcPkgMilestoneEvent.IsAccepted, deserializedResult.IsAccepted);
        Assert.AreEqual(mcPkgMilestoneEvent.IsRejected, deserializedResult.IsRejected);
        Assert.AreEqual(mcPkgMilestoneEvent.LastUpdated, deserializedResult.LastUpdated);
        Assert.AreEqual(mcPkgMilestoneEvent.EventType, deserializedResult.EventType);
    }

    [TestMethod]
    public async Task CreatePipingRevisionMessage_ValidMessage_ReturnsSerializedPipingRevisionEvent()
    {
        // Arrange
        const string message = "12345";
        const long revisionId = 12345L;
        var query = PipingRevisionQuery.GetQuery(revisionId);

        var pipingRevisionEvent = new PipingRevisionEvent
        {
            Plant = "Plant1",
            ProCoSysGuid = Guid.NewGuid(),
            PipingRevisionId = 1,
            Revision = 2,
            McPkgNo = "McPkg1",
            McPkgNoGuid = Guid.NewGuid(),
            ProjectName = "Project1",
            MaxDesignPressure = 100.0,
            MaxTestPressure = 150.0,
            Comments = "Sample comments",
            TestISODocumentNo = "Doc123",
            TestISODocumentGuid = Guid.NewGuid(),
            TestISORevision = 1,
            PurchaseOrderNo = "PO123",
            CallOffNo = "CO123",
            CallOffGuid = Guid.NewGuid(),
            LastUpdated = DateTime.UtcNow
        };

        _dapperRepositoryMock.Setup(repo => repo.QuerySingle<PipingRevisionEvent>(
                It.Is<(string, DynamicParameters)>( qs => qs.Item1 == query.queryString),
                message))
            .ReturnsAsync(pipingRevisionEvent);

        // Act
        var result = await _dut.CreatePipingRevisionMessage(message);

        // Assert
        Assert.IsNotNull(result);
        var deserializedResult =
            JsonSerializer.Deserialize<PipingRevisionEvent>(result, DefaultSerializerHelper.SerializerOptions);

        // Check if the properties are equal
        Assert.AreEqual(pipingRevisionEvent.Plant, deserializedResult.Plant);
        Assert.AreEqual(pipingRevisionEvent.ProCoSysGuid, deserializedResult.ProCoSysGuid);
        Assert.AreEqual(pipingRevisionEvent.PipingRevisionId, deserializedResult.PipingRevisionId);
        Assert.AreEqual(pipingRevisionEvent.Revision, deserializedResult.Revision);
        Assert.AreEqual(pipingRevisionEvent.McPkgNo, deserializedResult.McPkgNo);
        Assert.AreEqual(pipingRevisionEvent.McPkgNoGuid, deserializedResult.McPkgNoGuid);
        Assert.AreEqual(pipingRevisionEvent.ProjectName, deserializedResult.ProjectName);
        Assert.AreEqual(pipingRevisionEvent.MaxDesignPressure, deserializedResult.MaxDesignPressure);
        Assert.AreEqual(pipingRevisionEvent.MaxTestPressure, deserializedResult.MaxTestPressure);
        Assert.AreEqual(pipingRevisionEvent.Comments, deserializedResult.Comments);
        Assert.AreEqual(pipingRevisionEvent.TestISODocumentNo, deserializedResult.TestISODocumentNo);
        Assert.AreEqual(pipingRevisionEvent.TestISODocumentGuid, deserializedResult.TestISODocumentGuid);
        Assert.AreEqual(pipingRevisionEvent.TestISORevision, deserializedResult.TestISORevision);
        Assert.AreEqual(pipingRevisionEvent.PurchaseOrderNo, deserializedResult.PurchaseOrderNo);
        Assert.AreEqual(pipingRevisionEvent.CallOffNo, deserializedResult.CallOffNo);
        Assert.AreEqual(pipingRevisionEvent.CallOffGuid, deserializedResult.CallOffGuid);
        Assert.AreEqual(pipingRevisionEvent.LastUpdated, deserializedResult.LastUpdated);
        Assert.AreEqual(pipingRevisionEvent.EventType, deserializedResult.EventType);
    }

    [TestMethod]
    public async Task CreatePipingSpoolMessage_ValidMessage_ReturnsSerializedPipingSpoolEvent()
    {
        // Arrange
        const string message = "12345";
        const long spoolId = 12345L;
        var query = PipingSpoolQuery.GetQuery(spoolId);

        var pipingSpoolEvent = new PipingSpoolEvent
        {
            Plant = "Plant1",
            ProCoSysGuid = Guid.NewGuid(),
            ProjectName = "Project1",
            PipingSpoolId = 1,
            PipingRevisionId = 1,
            PipingRevisionGuid = Guid.NewGuid(),
            Revision = 2,
            McPkgNo = "McPkg1",
            McPkgGuid = Guid.NewGuid(),
            ISODrawing = "ISO123",
            Spool = "Spool1",
            LineNo = "Line1",
            LineGuid = Guid.NewGuid(),
            N2HeTest = true,
            AlternativeTest = false,
            AlternativeTestNoOfWelds = 10,
            Installed = true,
            Welded = true,
            WeldedDate = DateOnly.FromDateTime(DateTime.UtcNow),
            PressureTested = true,
            NDE = false,
            Primed = true,
            Painted = true,
            LastUpdated = DateTime.UtcNow
        };

        _dapperRepositoryMock.Setup(repo => repo.QuerySingle<PipingSpoolEvent>(
                It.Is<(string, DynamicParameters)>( qs => qs.Item1 == query.queryString),
                message))
            .ReturnsAsync(pipingSpoolEvent);

        // Act
        var result = await _dut.CreatePipingSpoolMessage(message);

        // Assert
        Assert.IsNotNull(result);
        var deserializedResult =
            JsonSerializer.Deserialize<PipingSpoolEvent>(result, DefaultSerializerHelper.SerializerOptions);

        // Check if the properties are equal
        Assert.AreEqual(pipingSpoolEvent.Plant, deserializedResult.Plant);
        Assert.AreEqual(pipingSpoolEvent.ProCoSysGuid, deserializedResult.ProCoSysGuid);
        Assert.AreEqual(pipingSpoolEvent.ProjectName, deserializedResult.ProjectName);
        Assert.AreEqual(pipingSpoolEvent.PipingSpoolId, deserializedResult.PipingSpoolId);
        Assert.AreEqual(pipingSpoolEvent.PipingRevisionId, deserializedResult.PipingRevisionId);
        Assert.AreEqual(pipingSpoolEvent.PipingRevisionGuid, deserializedResult.PipingRevisionGuid);
        Assert.AreEqual(pipingSpoolEvent.Revision, deserializedResult.Revision);
        Assert.AreEqual(pipingSpoolEvent.McPkgNo, deserializedResult.McPkgNo);
        Assert.AreEqual(pipingSpoolEvent.McPkgGuid, deserializedResult.McPkgGuid);
        Assert.AreEqual(pipingSpoolEvent.ISODrawing, deserializedResult.ISODrawing);
        Assert.AreEqual(pipingSpoolEvent.Spool, deserializedResult.Spool);
        Assert.AreEqual(pipingSpoolEvent.LineNo, deserializedResult.LineNo);
        Assert.AreEqual(pipingSpoolEvent.LineGuid, deserializedResult.LineGuid);
        Assert.AreEqual(pipingSpoolEvent.N2HeTest, deserializedResult.N2HeTest);
        Assert.AreEqual(pipingSpoolEvent.AlternativeTest, deserializedResult.AlternativeTest);
        Assert.AreEqual(pipingSpoolEvent.AlternativeTestNoOfWelds, deserializedResult.AlternativeTestNoOfWelds);
        Assert.AreEqual(pipingSpoolEvent.Installed, deserializedResult.Installed);
        Assert.AreEqual(pipingSpoolEvent.Welded, deserializedResult.Welded);
        Assert.AreEqual(pipingSpoolEvent.WeldedDate, deserializedResult.WeldedDate);
        Assert.AreEqual(pipingSpoolEvent.PressureTested, deserializedResult.PressureTested);
        Assert.AreEqual(pipingSpoolEvent.NDE, deserializedResult.NDE);
        Assert.AreEqual(pipingSpoolEvent.Primed, deserializedResult.Primed);
        Assert.AreEqual(pipingSpoolEvent.Painted, deserializedResult.Painted);
        Assert.AreEqual(pipingSpoolEvent.LastUpdated, deserializedResult.LastUpdated);
        Assert.AreEqual(pipingSpoolEvent.EventType, deserializedResult.EventType);
    }

    [TestMethod]
    public async Task CreateProjectMessage_ValidMessage_ReturnsSerializedProjectEvent()
    {
        // Arrange
        const string message = "12345";
        const long projectId = 12345L;
        var query = ProjectQuery.GetQuery(projectId);

        var projectEvent = new ProjectEvent
        {
            Plant = "Plant1",
            ProCoSysGuid = Guid.NewGuid(),
            ProjectName = "Project1",
            IsClosed = false,
            Description = "Test project",
            LastUpdated = DateTime.UtcNow
        };

        _dapperRepositoryMock.Setup(repo => repo.QuerySingle<ProjectEvent>(
                It.Is<(string, DynamicParameters)>( qs => qs.Item1 == query.queryString),
                message))
            .ReturnsAsync(projectEvent);

        // Act
        var result = await _dut.CreateProjectMessage(message);

        // Assert
        Assert.IsNotNull(result);
        var deserializedResult =
            JsonSerializer.Deserialize<ProjectEvent>(result, DefaultSerializerHelper.SerializerOptions);

        // Check if the properties are equal
        Assert.AreEqual(projectEvent.Plant, deserializedResult.Plant);
        Assert.AreEqual(projectEvent.ProCoSysGuid, deserializedResult.ProCoSysGuid);
        Assert.AreEqual(projectEvent.ProjectName, deserializedResult.ProjectName);
        Assert.AreEqual(projectEvent.IsClosed, deserializedResult.IsClosed);
        Assert.AreEqual(projectEvent.Description, deserializedResult.Description);
        Assert.AreEqual(projectEvent.LastUpdated, deserializedResult.LastUpdated);
        Assert.AreEqual(projectEvent.EventType, deserializedResult.EventType);
    }

    [TestMethod]
    public async Task CreatePunchListItem_ValidMessage_ReturnsSerializedPunchListItemEvent()
    {
        // Arrange
        const string message = "12345";
        const long punchListItemId = 12345L;
        var query = PunchListItemQuery.GetQuery(punchListItemId);

        var punchListItemEvent = new PunchListItemEvent
        {
            Plant = "Plant1",
            ProCoSysGuid = Guid.NewGuid(),
            ProjectName = "Project1",
            LastUpdated = DateTime.UtcNow,
            PunchItemNo = 12345,
            Description = "Test punch item",
            ChecklistId = 67890,
            ChecklistGuid = Guid.NewGuid(),
            Category = "Category1",
            RaisedByOrg = "ORG1",
            ClearingByOrg = "ORG2",
            DueDate = DateTime.UtcNow.AddDays(10),
            PunchListSorting = "Sort1",
            PunchListType = "Type1",
            PunchPriority = "P1",
            Estimate = "Estimate1",
            OriginalWoNo = "WO1",
            OriginalWoGuid = Guid.NewGuid(),
            WoNo = "WO2",
            WoGuid = Guid.NewGuid(),
            SWCRNo = "SWCR1",
            SWCRGuid = Guid.NewGuid(),
            DocumentNo = "DOC1",
            DocumentGuid = Guid.NewGuid(),
            ExternalItemNo = "EXT1",
            MaterialRequired = true,
            IsVoided = false,
            MaterialETA = DateTime.UtcNow.AddDays(5),
            MaterialExternalNo = "EXT2",
            ClearedAt = DateTime.UtcNow.AddDays(-2),
            RejectedAt = null,
            VerifiedAt = null,
            CreatedAt = DateTime.UtcNow.AddDays(-7)
        };

        _dapperRepositoryMock.Setup(repo => repo.QuerySingle<PunchListItemEvent>(
                It.Is<(string, DynamicParameters)>( qs => qs.Item1 == query.queryString),
                message))
            .ReturnsAsync(punchListItemEvent);

        // Act
        var result = await _dut.CreatePunchListItemMessage(message);

        // Assert
        Assert.IsNotNull(result);
        var deserializedResult =
            JsonSerializer.Deserialize<PunchListItemEvent>(result);

        // Check if the properties are equal
        Assert.AreEqual(punchListItemEvent.Plant, deserializedResult.Plant);
        Assert.AreEqual(punchListItemEvent.ProCoSysGuid, deserializedResult.ProCoSysGuid);
        Assert.AreEqual(punchListItemEvent.ProjectName, deserializedResult.ProjectName);
        Assert.AreEqual(punchListItemEvent.LastUpdated, deserializedResult.LastUpdated);
        Assert.AreEqual(punchListItemEvent.PunchItemNo, deserializedResult.PunchItemNo);
        Assert.AreEqual(punchListItemEvent.Description, deserializedResult.Description);
        Assert.AreEqual(punchListItemEvent.ChecklistId, deserializedResult.ChecklistId);
        Assert.AreEqual(punchListItemEvent.ChecklistGuid, deserializedResult.ChecklistGuid);
        Assert.AreEqual(punchListItemEvent.Category, deserializedResult.Category);
        Assert.AreEqual(punchListItemEvent.RaisedByOrg, deserializedResult.RaisedByOrg);
        Assert.AreEqual(punchListItemEvent.ClearingByOrg, deserializedResult.ClearingByOrg);
        Assert.AreEqual(punchListItemEvent.DueDate, deserializedResult.DueDate);
        Assert.AreEqual(punchListItemEvent.PunchListSorting, deserializedResult.PunchListSorting);
        Assert.AreEqual(punchListItemEvent.PunchListType, deserializedResult.PunchListType);
        Assert.AreEqual(punchListItemEvent.PunchPriority, deserializedResult.PunchPriority);
        Assert.AreEqual(punchListItemEvent.Estimate, deserializedResult.Estimate);
        Assert.AreEqual(punchListItemEvent.OriginalWoNo, deserializedResult.OriginalWoNo);
        Assert.AreEqual(punchListItemEvent.OriginalWoGuid, deserializedResult.OriginalWoGuid);
        Assert.AreEqual(punchListItemEvent.WoNo, deserializedResult.WoNo);
        Assert.AreEqual(punchListItemEvent.WoGuid, deserializedResult.WoGuid);
        Assert.AreEqual(punchListItemEvent.SWCRNo, deserializedResult.SWCRNo);
        Assert.AreEqual(punchListItemEvent.SWCRGuid, deserializedResult.SWCRGuid);
        Assert.AreEqual(punchListItemEvent.DocumentNo, deserializedResult.DocumentNo);
        Assert.AreEqual(punchListItemEvent.DocumentGuid, deserializedResult.DocumentGuid);
        Assert.AreEqual(deserializedResult.ExternalItemNo, deserializedResult.ExternalItemNo);
        Assert.AreEqual(punchListItemEvent.MaterialRequired, deserializedResult.MaterialRequired);
        Assert.AreEqual(punchListItemEvent.IsVoided, deserializedResult.IsVoided);
        Assert.AreEqual(punchListItemEvent.MaterialETA, deserializedResult.MaterialETA);
        Assert.AreEqual(punchListItemEvent.MaterialExternalNo, deserializedResult.MaterialExternalNo);
        Assert.AreEqual(punchListItemEvent.ClearedAt, deserializedResult.ClearedAt);
        Assert.AreEqual(punchListItemEvent.RejectedAt, deserializedResult.RejectedAt);
        Assert.AreEqual(punchListItemEvent.VerifiedAt, deserializedResult.VerifiedAt);
        Assert.AreEqual(punchListItemEvent.CreatedAt, deserializedResult.CreatedAt);
    }

    [TestMethod]
    public async Task CreateQueryMessage_ValidMessage_ReturnsSerializedQueryEvent()
    {
        // Arrange
        const string message = "12345";
        const long queryId = 12345L;
        var query = QueryQuery.GetQuery(queryId);
        var queryEvent = new QueryEvent
        {
            Plant = "GardenOfMystery",
            ProCoSysGuid = Guid.NewGuid(),
            ProjectName = "EnigmaticExpedition",
            QueryId = 42,
            QueryNo = "R-1",
            Title = "The Sphinx's Secret",
            DisciplineCode = "ARCH",
            QueryType = "Riddle",
            CostImpact = "Medium",
            Description = "What walks on four legs in the morning, two legs at noon, and three legs in the evening?",
            Consequence = "Failure to solve the riddle may result in dire consequences.",
            ProposedSolution = "The answer is a human being.",
            EngineeringReply = "Solution accepted, the riddle is solved.",
            Milestone = "M2",
            ScheduleImpact = false,
            PossibleWarrantyClaim = false,
            IsVoided = false,
            RequiredDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(3)),
            CreatedAt = DateTime.UtcNow,
            LastUpdated = DateTime.UtcNow
        };

        _dapperRepositoryMock.Setup(repo => repo.QuerySingle<QueryEvent>(
                It.Is<(string, DynamicParameters)>( qs => qs.Item1 == query.queryString),
                message))
            .ReturnsAsync(queryEvent);

        // Act
        var result = await _dut.CreateQueryMessage(message);

        // Assert
        Assert.IsNotNull(result);
        var deserializedResult =
            JsonSerializer.Deserialize<QueryEvent>(result, DefaultSerializerHelper.SerializerOptions);

        // Check if the properties are equal
        Assert.IsNotNull(deserializedResult);
        Assert.AreEqual(queryEvent.Plant, deserializedResult.Plant);
        Assert.AreEqual(queryEvent.ProCoSysGuid, deserializedResult.ProCoSysGuid);
        Assert.AreEqual(queryEvent.ProjectName, deserializedResult.ProjectName);
        Assert.AreEqual(queryEvent.QueryId, deserializedResult.QueryId);
        Assert.AreEqual(queryEvent.QueryNo, deserializedResult.QueryNo);
        Assert.AreEqual(queryEvent.Title, deserializedResult.Title);
        Assert.AreEqual(queryEvent.DisciplineCode, deserializedResult.DisciplineCode);
        Assert.AreEqual(queryEvent.QueryType, deserializedResult.QueryType);
        Assert.AreEqual(queryEvent.CostImpact, deserializedResult.CostImpact);
        Assert.AreEqual(queryEvent.Description, deserializedResult.Description);
        Assert.AreEqual(queryEvent.Consequence, deserializedResult.Consequence);
        Assert.AreEqual(queryEvent.ProposedSolution, deserializedResult.ProposedSolution);
        Assert.AreEqual(queryEvent.EngineeringReply, deserializedResult.EngineeringReply);
        Assert.AreEqual(queryEvent.Milestone, deserializedResult.Milestone);
        Assert.AreEqual(queryEvent.ScheduleImpact, deserializedResult.ScheduleImpact);
        Assert.AreEqual(queryEvent.PossibleWarrantyClaim, deserializedResult.PossibleWarrantyClaim);
        Assert.AreEqual(queryEvent.IsVoided, deserializedResult.IsVoided);
        Assert.AreEqual(queryEvent.RequiredDate, deserializedResult.RequiredDate);
        Assert.AreEqual(queryEvent.CreatedAt, deserializedResult.CreatedAt);
        Assert.AreEqual(queryEvent.LastUpdated, deserializedResult.LastUpdated);
    }

    [TestMethod]
    public async Task CreateQuerySignatureMessage_ValidMessage_ReturnsSerializedQuerySignatureEvent()
    {
        // Arrange
        const string message = "12345";
        const long queryId = 12345L;
        var query = QuerySignatureQuery.GetQuery(queryId);
        var querySignatureEvent = new QuerySignatureEvent
        {
            Plant = "GardenOfMystery",
            ProCoSysGuid = Guid.NewGuid(),
            ProjectName = "EnigmaticExpedition",
            QueryId = 42,
            QueryNo = "R-1",
            LastUpdated = DateTime.UtcNow
        };

        _dapperRepositoryMock.Setup(repo => repo.QuerySingle<QuerySignatureEvent>(
                It.Is<(string, DynamicParameters)>( qs => qs.Item1 == query.queryString),
                message))
            .ReturnsAsync(querySignatureEvent);

        // Act
        var result = await _dut.CreateQuerySignatureMessage(message);

        // Assert
        Assert.IsNotNull(result);
        var deserializedResult =
            JsonSerializer.Deserialize<QuerySignatureEvent>(result, DefaultSerializerHelper.SerializerOptions);

        // Check if the properties are equal
        Assert.IsNotNull(deserializedResult);
        Assert.AreEqual(querySignatureEvent.Plant, deserializedResult.Plant);
        Assert.AreEqual(querySignatureEvent.ProCoSysGuid, deserializedResult.ProCoSysGuid);
        Assert.AreEqual(querySignatureEvent.ProjectName, deserializedResult.ProjectName);
        Assert.AreEqual(querySignatureEvent.QueryId, deserializedResult.QueryId);
        Assert.AreEqual(querySignatureEvent.QueryNo, deserializedResult.QueryNo);

        Assert.AreEqual(querySignatureEvent.LastUpdated, deserializedResult.LastUpdated);
    }

    [TestMethod]
    public async Task CreateResponsibleMessage_ValidMessage_ReturnsSerializedResponsibleEvent()
    {
        // Arrange
        const string message = "12345";
        const long responsibleId = 12345L;
        var query = ResponsibleQuery.GetQuery(responsibleId);
        var responsibleEvent = new ResponsibleEvent
        {
            Plant = "MysteriousIsland",
            ProCoSysGuid = Guid.NewGuid(),
            ResponsibleId = 101,
            Code = "SHR",
            ResponsibleGroup = "Sherlock",
            Description = "Solving enigmatic cases",
            IsVoided = false,
            LastUpdated = DateTime.UtcNow
        };

        _dapperRepositoryMock.Setup(repo => repo.QuerySingle<ResponsibleEvent>(
                It.Is<(string, DynamicParameters)>( qs => qs.Item1 == query.queryString),
                message))
            .ReturnsAsync(responsibleEvent);

        // Act
        var result = await _dut.CreateResponsibleMessage(message);

        // Assert
        Assert.IsNotNull(result);
        var deserializedResult =
            JsonSerializer.Deserialize<ResponsibleEvent>(result, DefaultSerializerHelper.SerializerOptions);

        // Check if the properties are equal
        Assert.IsNotNull(responsibleEvent);
        Assert.AreEqual(responsibleEvent.Plant, deserializedResult.Plant);
        Assert.AreEqual(responsibleEvent.ProCoSysGuid, deserializedResult.ProCoSysGuid);
        Assert.AreEqual(responsibleEvent.ResponsibleId, deserializedResult.ResponsibleId);
        Assert.AreEqual(responsibleEvent.Code, deserializedResult.Code);
        Assert.AreEqual(responsibleEvent.ResponsibleGroup, deserializedResult.ResponsibleGroup);
        Assert.AreEqual(responsibleEvent.Description, deserializedResult.Description);
        Assert.AreEqual(responsibleEvent.IsVoided, deserializedResult.IsVoided);
        Assert.AreEqual(responsibleEvent.LastUpdated.Date, deserializedResult.LastUpdated.Date);
    }

    [TestMethod]
    public async Task CreateStockMessage_ValidMessage_ReturnsSerializedStockEvent()
    {
        // Arrange
        const string message = "12345";
        const long stockId = 12345L;
        var query = StockQuery.GetQuery(stockId);
        var stockEvent = new StockEvent
        {
            Plant = "EnchantedForest",
            ProCoSysGuid = Guid.NewGuid(),
            StockId = 42,
            StockNo = "MAGIC001",
            Description = "Magical artifacts",
            LastUpdated = DateTime.UtcNow
        };

        _dapperRepositoryMock.Setup(repo => repo.QuerySingle<StockEvent>(
                It.Is<(string, DynamicParameters)>( qs => qs.Item1 == query.queryString),
                message))
            .ReturnsAsync(stockEvent);

        // Act
        var result = await _dut.CreateStockMessage(message);

        // Assert
        Assert.IsNotNull(result);
        var deserializedResult =
            JsonSerializer.Deserialize<StockEvent>(result, DefaultSerializerHelper.SerializerOptions);

        Assert.AreEqual(stockEvent.Plant, deserializedResult.Plant);
        Assert.AreEqual(stockEvent.ProCoSysGuid, deserializedResult.ProCoSysGuid);
        Assert.AreEqual(stockEvent.StockId, deserializedResult.StockId);
        Assert.AreEqual(stockEvent.StockNo, deserializedResult.StockNo);
        Assert.AreEqual(stockEvent.Description, deserializedResult.Description);
        Assert.AreEqual(stockEvent.LastUpdated, deserializedResult.LastUpdated);
    }

    [TestMethod]
    public async Task CreateSwcrAttachmentMessage_ValidMessage_ReturnsSerializedSwcrAttachmentEvent()
    {
        // Arrange
        const string message = "5f643eeb-b114-4fc4-b884-ade3f6ea63ce";
        var query = SwcrAttachmentQuery.GetQuery(message);
        var swcrAttachmentEvent = new SwcrAttachmentEvent
        {
            Plant = "EnchantedForest",
            ProCoSysGuid = Guid.NewGuid(),
            SwcrGuid = Guid.NewGuid(),
            Title = "MysteriousArtifact",
            ClassificationCode = "MAGIC",
            Uri = "https://example.com/mysterious-artifact",
            FileName = "mysterious-artifact.jpg",
            LastUpdated = DateTime.UtcNow
        };

        _dapperRepositoryMock.Setup(repo => repo.QuerySingle<SwcrAttachmentEvent>(
                It.Is<(string, DynamicParameters)>( qs => qs.Item1 == query.queryString),
                message))
            .ReturnsAsync(swcrAttachmentEvent);

        // Act
        var result = await _dut.CreateSwcrAttachmentMessage(message);

        // Assert
        Assert.IsNotNull(result);
        var deserializedResult =
            JsonSerializer.Deserialize<SwcrAttachmentEvent>(result, DefaultSerializerHelper.SerializerOptions);

        Assert.AreEqual(swcrAttachmentEvent.Plant, deserializedResult.Plant);
        Assert.AreEqual(swcrAttachmentEvent.ProCoSysGuid, deserializedResult.ProCoSysGuid);
        Assert.AreEqual(swcrAttachmentEvent.SwcrGuid, deserializedResult.SwcrGuid);
        Assert.AreEqual(swcrAttachmentEvent.Title, deserializedResult.Title);
        Assert.AreEqual(swcrAttachmentEvent.ClassificationCode, deserializedResult.ClassificationCode);
        Assert.AreEqual(swcrAttachmentEvent.Uri, deserializedResult.Uri);
        Assert.AreEqual(swcrAttachmentEvent.FileName, deserializedResult.FileName);
        Assert.AreEqual(swcrAttachmentEvent.LastUpdated, deserializedResult.LastUpdated);
    }

    [TestMethod]
    public async Task CreateSwcrMessage_ValidMessage_ReturnsSerializedSwcrEvent()
    {
        // Arrange
        const string message = "12345";
        const long swcrId = 12345L;
        var query = SwcrQuery.GetQuery(swcrId);
        var swcrEvent = new SwcrEvent
        {
            Plant = "Plant1",
            ProCoSysGuid = Guid.NewGuid(),
            ProjectName = "ProjectName1",
            SwcrNo = "SWCRNO1",
            SwcrId = 1,
            CommPkgGuid = Guid.NewGuid(),
            CommPkgNo = "CommPkgNo1",
            Description = "Description1",
            Modification = "Modification1",
            Priority = "Priority1",
            System = "System1",
            ControlSystem = "ControlSystem1",
            Contract = "Contract1",
            Supplier = "Supplier1",
            Node = "Node1",
            Status = "Status1",
            CreatedAt = DateTime.UtcNow,
            IsVoided = false,
            LastUpdated = DateTime.UtcNow,
            DueDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10)),
            EstimatedManHours = 100.0f
        };

        _dapperRepositoryMock.Setup(repo => repo.QuerySingle<SwcrEvent>(
                It.Is<(string, DynamicParameters)>( qs => qs.Item1 == query.queryString),
                message))
            .ReturnsAsync(swcrEvent);

        // Act
        var result = await _dut.CreateSwcrMessage(message);

        // Assert
        Assert.IsNotNull(result);
        var deserializedResult =
            JsonSerializer.Deserialize<SwcrEvent>(result, DefaultSerializerHelper.SerializerOptions);

        // Check if the properties are equal
        Assert.AreEqual(swcrEvent.Plant, deserializedResult.Plant);
        Assert.AreEqual(swcrEvent.ProCoSysGuid, deserializedResult.ProCoSysGuid);
        Assert.AreEqual(swcrEvent.ProjectName, deserializedResult.ProjectName);
        Assert.AreEqual(swcrEvent.SwcrNo, deserializedResult.SwcrNo);
        Assert.AreEqual(swcrEvent.SwcrId, deserializedResult.SwcrId);
        Assert.AreEqual(swcrEvent.CommPkgGuid, deserializedResult.CommPkgGuid);
        Assert.AreEqual(swcrEvent.CommPkgNo, deserializedResult.CommPkgNo);
        Assert.AreEqual(swcrEvent.Description, deserializedResult.Description);
        Assert.AreEqual(swcrEvent.Modification, deserializedResult.Modification);
        Assert.AreEqual(swcrEvent.Priority, deserializedResult.Priority);
        Assert.AreEqual(swcrEvent.System, deserializedResult.System);
        Assert.AreEqual(swcrEvent.ControlSystem, deserializedResult.ControlSystem);
        Assert.AreEqual(swcrEvent.Contract, deserializedResult.Contract);
        Assert.AreEqual(swcrEvent.Supplier, deserializedResult.Supplier);
        Assert.AreEqual(swcrEvent.Node, deserializedResult.Node);
        Assert.AreEqual(swcrEvent.Status, deserializedResult.Status);
        Assert.AreEqual(swcrEvent.CreatedAt, deserializedResult.CreatedAt);
        Assert.AreEqual(swcrEvent.IsVoided, deserializedResult.IsVoided);
        Assert.AreEqual(swcrEvent.LastUpdated, deserializedResult.LastUpdated);
        Assert.AreEqual(swcrEvent.DueDate, deserializedResult.DueDate);
        Assert.AreEqual(swcrEvent.EstimatedManHours, deserializedResult.EstimatedManHours);
        Assert.AreEqual(swcrEvent.EventType, deserializedResult.EventType);
    }

    [TestMethod]
    public async Task CreateSwcrOtherReferenceMessage_ValidMessage_ReturnsSerializedSwcrOtherReferenceEvent()
    {
        // Arrange
        const string message = "5f643eeb-0b7a-4b9e-9b1a-0e6b7b7b6b6b";
        var query = SwcrOtherReferenceQuery.GetQuery(message);
        var swcrOtherReferenceEvent = new SwcrOtherReferenceEvent
        {
            Plant = "EnchantedForest",
            ProCoSysGuid = Guid.NewGuid(),
            LibraryGuid = Guid.NewGuid(),
            SwcrGuid = Guid.NewGuid(),
            Code = "MAGIC_REF",
            Description = "Mysterious artifact reference",
            LastUpdated = DateTime.UtcNow
        };

        _dapperRepositoryMock.Setup(repo => repo.QuerySingle<SwcrOtherReferenceEvent>(
                It.Is<(string, DynamicParameters)>( qs => qs.Item1 == query.queryString),
                message))
            .ReturnsAsync(swcrOtherReferenceEvent);

        // Act
        var result = await _dut.CreateSwcrOtherReferenceMessage(message);

        // Assert
        Assert.IsNotNull(result);
        var deserializedResult =
            JsonSerializer.Deserialize<SwcrOtherReferenceEvent>(result, DefaultSerializerHelper.SerializerOptions);

        // Check if the properties are equal
        Assert.AreEqual(swcrOtherReferenceEvent.Plant, deserializedResult.Plant);
        Assert.AreEqual(swcrOtherReferenceEvent.ProCoSysGuid, deserializedResult.ProCoSysGuid);
        Assert.AreEqual(swcrOtherReferenceEvent.LibraryGuid, deserializedResult.LibraryGuid);
        Assert.AreEqual(swcrOtherReferenceEvent.SwcrGuid, deserializedResult.SwcrGuid);
        Assert.AreEqual(swcrOtherReferenceEvent.Code, deserializedResult.Code);
        Assert.AreEqual(swcrOtherReferenceEvent.Description, deserializedResult.Description);
        Assert.AreEqual(swcrOtherReferenceEvent.LastUpdated, deserializedResult.LastUpdated);
    }

    [TestMethod]
    public async Task CreateSwcrSignatureMessage_ValidMessage_ReturnsSerializedSwcrSignatureEvent()
    {
        // Arrange
        const string message = "1234";
        var swcrSignatureId = long.Parse(message);
        var query = SwcrSignatureQuery.GetQuery(swcrSignatureId);
        var swcrSignatureEvent = new SwcrSignatureEvent
        {
            Plant = "EnchantedForest",
            ProCoSysGuid = Guid.NewGuid(),
            SwcrSignatureId = 12345,
            ProjectName = "MagicalProject",
            SwcrNo = "SWCR-001",
            SwcrGuid = Guid.NewGuid(),
            SignatureRoleCode = "WIZARD",
            SignatureRoleDescription = "Wizard's approval",
            Sequence = 1,
            SignedByAzureOid = Guid.NewGuid(),
            FunctionalRoleCode = "MAGIC_ROLE",
            FunctionalRoleDescription = "Magic-related tasks",
            SignedDate = DateTime.UtcNow,
            LastUpdated = DateTime.UtcNow
        };

        _dapperRepositoryMock.Setup(repo => repo.QuerySingle<SwcrSignatureEvent>(
                It.Is<(string, DynamicParameters)>( qs => qs.Item1 == query.queryString),
                message))
            .ReturnsAsync(swcrSignatureEvent);

        // Act
        var result = await _dut.CreateSwcrSignatureMessage(message);

        // Assert
        Assert.IsNotNull(result);
        var deserializedResult =
            JsonSerializer.Deserialize<SwcrSignatureEvent>(result, DefaultSerializerHelper.SerializerOptions);

        // Check if the properties are equal
        Assert.AreEqual(swcrSignatureEvent.Plant, deserializedResult.Plant);
        Assert.AreEqual(swcrSignatureEvent.ProCoSysGuid, deserializedResult.ProCoSysGuid);
        Assert.AreEqual(swcrSignatureEvent.SwcrSignatureId, deserializedResult.SwcrSignatureId);
        Assert.AreEqual(swcrSignatureEvent.ProjectName, deserializedResult.ProjectName);
        Assert.AreEqual(swcrSignatureEvent.SwcrNo, deserializedResult.SwcrNo);
        Assert.AreEqual(swcrSignatureEvent.SwcrGuid, deserializedResult.SwcrGuid);
        Assert.AreEqual(swcrSignatureEvent.SignatureRoleCode, deserializedResult.SignatureRoleCode);
        Assert.AreEqual(swcrSignatureEvent.SignatureRoleDescription, deserializedResult.SignatureRoleDescription);
        Assert.AreEqual(swcrSignatureEvent.Sequence, deserializedResult.Sequence);
        Assert.AreEqual(swcrSignatureEvent.SignedByAzureOid, deserializedResult.SignedByAzureOid);
        Assert.AreEqual(swcrSignatureEvent.FunctionalRoleCode, deserializedResult.FunctionalRoleCode);
        Assert.AreEqual(swcrSignatureEvent.FunctionalRoleDescription, deserializedResult.FunctionalRoleDescription);
        Assert.AreEqual(swcrSignatureEvent.SignedDate, deserializedResult.SignedDate);
        Assert.AreEqual(swcrSignatureEvent.LastUpdated, deserializedResult.LastUpdated);
    }

    [TestMethod]
    public async Task CreateSwcrTypeMessage_ValidMessage_ReturnsSerializedSwcrTypeEvent()
    {
        // Arrange
        const string message = "5a643eeb-0b7a-4b9e-9b1a-0e6b7b7b6b6b";
        var query = SwcrTypeQuery.GetQuery(message);
        var swcrTypeEvent = new SwcrTypeEvent
        {
            Plant = "EnchantedForest",
            ProCoSysGuid = Guid.NewGuid(),
            LibraryGuid = Guid.NewGuid(),
            SwcrGuid = Guid.NewGuid(),
            Code = "MAGIC_TYPE",
            LastUpdated = DateTime.UtcNow
        };

        _dapperRepositoryMock.Setup(repo => repo.QuerySingle<SwcrTypeEvent>(
                It.Is<(string, DynamicParameters)>( qs => qs.Item1 == query.queryString),
                message))
            .ReturnsAsync(swcrTypeEvent);

        // Act
        var result = await _dut.CreateSwcrTypeMessage(message);

        // Assert
        Assert.IsNotNull(result);
        var deserializedResult =
            JsonSerializer.Deserialize<SwcrTypeEvent>(result, DefaultSerializerHelper.SerializerOptions);

        // Check if the properties are equal
        Assert.AreEqual(swcrTypeEvent.Plant, deserializedResult.Plant);
        Assert.AreEqual(swcrTypeEvent.ProCoSysGuid, deserializedResult.ProCoSysGuid);
        Assert.AreEqual(swcrTypeEvent.LibraryGuid, deserializedResult.LibraryGuid);
        Assert.AreEqual(swcrTypeEvent.SwcrGuid, deserializedResult.SwcrGuid);
        Assert.AreEqual(swcrTypeEvent.Code, deserializedResult.Code);
        Assert.AreEqual(swcrTypeEvent.LastUpdated, deserializedResult.LastUpdated);
    }

    [TestMethod]
    public async Task CreateTaskMessage_ValidMessage_ReturnsSerializedTaskEvent()
    {
        // Arrange
        const string message = "123456";
        var taskId = long.Parse(message);
        var query = TaskQuery.GetQuery(taskId);
        var taskEvent = new TaskEvent
        {
            Plant = "MysteriousIsland",
            ProCoSysGuid = Guid.NewGuid(),
            TaskParentProCoSysGuid = Guid.NewGuid(),
            ProjectName = "SecretProject",
            DocumentId = 123,
            Title = "HiddenTask",
            TaskId = "T123",
            ElementContentGuid = Guid.NewGuid(),
            Description = "Discover the secrets",
            Comments = "Be cautious",
            LastUpdated = DateTime.UtcNow,
            SignedAt = DateTime.UtcNow,
            SignedBy = Guid.NewGuid()
        };


        _dapperRepositoryMock.Setup(repo => repo.QuerySingle<TaskEvent>(
                It.Is<(string, DynamicParameters)>( qs => qs.Item1 == query.queryString),
                message))
            .ReturnsAsync(taskEvent);

        // Act
        var result = await _dut.CreateTaskMessage(message);

        // Assert
        Assert.IsNotNull(result);
        var deserializedResult =
            JsonSerializer.Deserialize<TaskEvent>(result, DefaultSerializerHelper.SerializerOptions);

        // Check if the properties are equal
        Assert.AreEqual(taskEvent.Plant, deserializedResult.Plant);
        Assert.AreEqual(taskEvent.ProCoSysGuid, deserializedResult.ProCoSysGuid);
        Assert.AreEqual(taskEvent.TaskParentProCoSysGuid, deserializedResult.TaskParentProCoSysGuid);
        Assert.AreEqual(taskEvent.ProjectName, deserializedResult.ProjectName);
        Assert.AreEqual(taskEvent.DocumentId, deserializedResult.DocumentId);
        Assert.AreEqual(taskEvent.Title, deserializedResult.Title);
        Assert.AreEqual(taskEvent.TaskId, deserializedResult.TaskId);
        Assert.AreEqual(taskEvent.ElementContentGuid, deserializedResult.ElementContentGuid);
        Assert.AreEqual(taskEvent.Description, deserializedResult.Description);
        Assert.AreEqual(taskEvent.Comments, deserializedResult.Comments);
        Assert.AreEqual(taskEvent.LastUpdated, deserializedResult.LastUpdated);
        Assert.AreEqual(taskEvent.SignedAt, deserializedResult.SignedAt);
        Assert.AreEqual(taskEvent.SignedBy, deserializedResult.SignedBy);
    }

    [TestMethod]
    public async Task CreateWorkOrderChecklistMessage_ValidMessage_ReturnsSerializedWorkOrderChecklistMessage()
    {
        // Arrange
        const string message = "4321,1234";
        const long workOrderId = 1234L;
        const long checkListId = 4321L;
        var query = WorkOrderChecklistQuery.GetQuery(checkListId, workOrderId);

        var workOrderChecklistEvent = new WorkOrderChecklistEvent
        {
            Plant = "EnigmaticFactory",
            ProCoSysGuid = Guid.NewGuid(),
            ProjectName = "HiddenProject",
            ChecklistId = 456,
            ChecklistGuid = Guid.NewGuid(),
            WoId = 789,
            WoGuid = Guid.NewGuid(),
            WoNo = "WO789",
            LastUpdated = DateTime.UtcNow
        };

        _dapperRepositoryMock.Setup(repo => repo.QuerySingle<WorkOrderChecklistEvent>(
                It.Is<(string, DynamicParameters)>( qs => qs.Item1 == query.queryString),
                message))
            .ReturnsAsync(workOrderChecklistEvent);

        // Act
        var result = await _dut.CreateWorkOrderChecklistMessage(message);

        // Assert
        Assert.IsNotNull(result);
        var deserializedResult =
            JsonSerializer.Deserialize<WorkOrderChecklistEvent>(result, DefaultSerializerHelper.SerializerOptions);

        // Check if the properties are equal
        Assert.AreEqual(workOrderChecklistEvent.Plant, deserializedResult.Plant);
        Assert.AreEqual(workOrderChecklistEvent.ProCoSysGuid, deserializedResult.ProCoSysGuid);
        Assert.AreEqual(workOrderChecklistEvent.ProjectName, deserializedResult.ProjectName);
        Assert.AreEqual(workOrderChecklistEvent.ChecklistId, deserializedResult.ChecklistId);
        Assert.AreEqual(workOrderChecklistEvent.ChecklistGuid, deserializedResult.ChecklistGuid);
        Assert.AreEqual(workOrderChecklistEvent.WoId, deserializedResult.WoId);
        Assert.AreEqual(workOrderChecklistEvent.WoGuid, deserializedResult.WoGuid);
        Assert.AreEqual(workOrderChecklistEvent.WoNo, deserializedResult.WoNo);
        Assert.AreEqual(workOrderChecklistEvent.LastUpdated, deserializedResult.LastUpdated);
    }

    [TestMethod]
    public async Task CreateWorkOrderCutoffMessage_ValidMessage_ReturnsSerializedWorkOrderCutoffMessage()
    {
        // Arrange
        const string message = "1234,03052023";
        const long workOrderId = 1234L;
        const string cutoffWeek = "03052023";
        var query = WorkOrderCutoffQuery.GetQuery(workOrderId, cutoffWeek);

        var workOrderCutoffEvent = new WorkOrderCutoffEvent
        {
            Plant = "MysteriousFactory",
            ProCoSysGuid = Guid.NewGuid(),
            PlantName = "HiddenPlant",
            WoGuid = Guid.NewGuid(),
            ProjectName = "SecretProject",
            WoNo = "WO123",
            JobStatusCode = "JS123",
            MaterialStatusCode = "MS123",
            DisciplineCode = "D123",
            CategoryCode = "C123",
            MilestoneCode = "M123",
            SubMilestoneCode = "SM123",
            HoldByCode = "HB123",
            PlanActivityCode = "PA123",
            ResponsibleCode = "R123",
            LastUpdated = DateTime.UtcNow,
            CutoffWeek = 1,
            CutoffDate = DateOnly.FromDateTime(DateTime.Now),
            PlannedStartAtDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
            PlannedFinishedAtDate = DateOnly.FromDateTime(DateTime.Now.AddDays(10)),
            ExpendedManHours = 100.0,
            ManHoursEarned = 80.0,
            EstimatedHours = 120.0,
            ManHoursExpendedLastWeek = 20.0,
            ManHoursEarnedLastWeek = 15.0,
            ProjectProgress = 75.0
        };

        _dapperRepositoryMock.Setup(repo => repo.QuerySingle<WorkOrderCutoffEvent>(
                It.Is<(string, DynamicParameters)>( qs => qs.Item1 == query.queryString),
                message))
            .ReturnsAsync(workOrderCutoffEvent);

        // Act
        var result = await _dut.CreateWorkOrderCutoffMessage(message);

        // Assert
        Assert.IsNotNull(result);
        var deserializedResult =
            JsonSerializer.Deserialize<WorkOrderCutoffEvent>(result, DefaultSerializerHelper.SerializerOptions);

        // Check if the properties are equal
        Assert.AreEqual(workOrderCutoffEvent.Plant, deserializedResult.Plant);
        Assert.AreEqual(workOrderCutoffEvent.ProCoSysGuid, deserializedResult.ProCoSysGuid);
        Assert.AreEqual(workOrderCutoffEvent.PlantName, deserializedResult.PlantName);
        Assert.AreEqual(workOrderCutoffEvent.WoGuid, deserializedResult.WoGuid);
        Assert.AreEqual(workOrderCutoffEvent.ProjectName, deserializedResult.ProjectName);
        Assert.AreEqual(workOrderCutoffEvent.WoNo, deserializedResult.WoNo);
        Assert.AreEqual(workOrderCutoffEvent.JobStatusCode, deserializedResult.JobStatusCode);
        Assert.AreEqual(workOrderCutoffEvent.MaterialStatusCode, deserializedResult.MaterialStatusCode);
        Assert.AreEqual(workOrderCutoffEvent.DisciplineCode, deserializedResult.DisciplineCode);
        Assert.AreEqual(workOrderCutoffEvent.CategoryCode, deserializedResult.CategoryCode);
        Assert.AreEqual(workOrderCutoffEvent.MilestoneCode, deserializedResult.MilestoneCode);
        Assert.AreEqual(workOrderCutoffEvent.SubMilestoneCode, deserializedResult.SubMilestoneCode);
        Assert.AreEqual(workOrderCutoffEvent.HoldByCode, deserializedResult.HoldByCode);
        Assert.AreEqual(workOrderCutoffEvent.PlanActivityCode, deserializedResult.PlanActivityCode);
        Assert.AreEqual(workOrderCutoffEvent.ResponsibleCode, deserializedResult.ResponsibleCode);
        Assert.AreEqual(workOrderCutoffEvent.LastUpdated, deserializedResult.LastUpdated);
        Assert.AreEqual(workOrderCutoffEvent.CutoffWeek, deserializedResult.CutoffWeek);
        Assert.AreEqual(workOrderCutoffEvent.CutoffDate, deserializedResult.CutoffDate);
        Assert.AreEqual(workOrderCutoffEvent.PlannedStartAtDate, deserializedResult.PlannedStartAtDate);
        Assert.AreEqual(workOrderCutoffEvent.PlannedFinishedAtDate, deserializedResult.PlannedFinishedAtDate);
        Assert.AreEqual(workOrderCutoffEvent.ExpendedManHours, deserializedResult.ExpendedManHours);
        Assert.AreEqual(workOrderCutoffEvent.ManHoursEarned, deserializedResult.ManHoursEarned);
        Assert.AreEqual(workOrderCutoffEvent.EstimatedHours, deserializedResult.EstimatedHours);
        Assert.AreEqual(workOrderCutoffEvent.ManHoursExpendedLastWeek, deserializedResult.ManHoursExpendedLastWeek);
        Assert.AreEqual(workOrderCutoffEvent.ManHoursEarnedLastWeek, deserializedResult.ManHoursEarnedLastWeek);
        Assert.AreEqual(workOrderCutoffEvent.ProjectProgress, deserializedResult.ProjectProgress);
    }

    [TestMethod]
    public async Task CreateWorkOrderMaterialMessage_ValidMessage_ReturnsSerializedWorkOrderMaterialMessage()
    {
        // Arrange
        const string message = "5a643eeb-0b7a-4b9e-9b1a-0e6b7b7b6b6b";

        var query = WorkOrderMaterialQuery.GetQuery(message);

        var workOrderMaterialEvent = new WorkOrderMaterialEvent
        {
            Plant = "ThePlant",
            ProCoSysGuid = Guid.NewGuid(),
            ProjectName = "TheEnigmaticProject",
            WoNo = "WONO",
            WoId = 12345,
            WoGuid = Guid.NewGuid(),
            ItemNo = "ITEMNO",
            TagNo = "TAGNO",
            TagId = 67890,
            TagGuid = Guid.NewGuid(),
            TagRegisterCode = "TRC",
            StockId = 24680,
            Quantity = 42.0,
            UnitName = "UN",
            UnitDescription = "\u003cp\u003eyeyeey\u003c/p\u003e",
            AdditionalInformation = "Additional Info",
            RequiredDate = DateOnly.FromDateTime(DateTime.Now.AddDays(10)),
            EstimatedAvailableDate = DateOnly.FromDateTime(DateTime.Now.AddDays(20)),
            Available = true,
            MaterialStatus = "MSTATUS",
            StockLocation = "SLOC",
            LastUpdated = DateTime.Now
        };

        _dapperRepositoryMock.Setup(repo => repo.QuerySingle<WorkOrderMaterialEvent>(
                It.Is<(string, DynamicParameters)>( qs => qs.Item1 == query.queryString),
                message))
            .ReturnsAsync(workOrderMaterialEvent);

        // Act
        var result = await _dut.CreateWorkOrderMaterialMessage(message);

        // Assert
        Assert.IsNotNull(result);
        var deserializedResult =
            JsonSerializer.Deserialize<WorkOrderMaterialEvent>(result, DefaultSerializerHelper.SerializerOptions);

        // Check if the properties are equal
        Assert.AreEqual(workOrderMaterialEvent.Plant, deserializedResult.Plant);
        Assert.AreEqual(workOrderMaterialEvent.ProCoSysGuid, deserializedResult.ProCoSysGuid);
        Assert.AreEqual(workOrderMaterialEvent.ProjectName, deserializedResult.ProjectName);
        Assert.AreEqual(workOrderMaterialEvent.WoNo, deserializedResult.WoNo);
        Assert.AreEqual(workOrderMaterialEvent.WoId, deserializedResult.WoId);
        Assert.AreEqual(workOrderMaterialEvent.WoGuid, deserializedResult.WoGuid);
        Assert.AreEqual(workOrderMaterialEvent.ItemNo, deserializedResult.ItemNo);
        Assert.AreEqual(workOrderMaterialEvent.TagNo, deserializedResult.TagNo);
        Assert.AreEqual(workOrderMaterialEvent.TagId, deserializedResult.TagId);
        Assert.AreEqual(workOrderMaterialEvent.TagGuid, deserializedResult.TagGuid);
        Assert.AreEqual(workOrderMaterialEvent.TagRegisterCode, deserializedResult.TagRegisterCode);
        Assert.AreEqual(workOrderMaterialEvent.StockId, deserializedResult.StockId);
        Assert.AreEqual(workOrderMaterialEvent.Quantity, deserializedResult.Quantity);
        Assert.AreEqual(workOrderMaterialEvent.UnitName, deserializedResult.UnitName);
        Assert.AreEqual(workOrderMaterialEvent.UnitDescription, deserializedResult.UnitDescription);
        Assert.AreEqual(workOrderMaterialEvent.AdditionalInformation, deserializedResult.AdditionalInformation);
        Assert.AreEqual(workOrderMaterialEvent.RequiredDate, deserializedResult.RequiredDate);
        Assert.AreEqual(workOrderMaterialEvent.EstimatedAvailableDate, deserializedResult.EstimatedAvailableDate);
        Assert.AreEqual(workOrderMaterialEvent.Available, deserializedResult.Available);
        Assert.AreEqual(workOrderMaterialEvent.MaterialStatus, deserializedResult.MaterialStatus);
        Assert.AreEqual(workOrderMaterialEvent.StockLocation, deserializedResult.StockLocation);
        Assert.AreEqual(workOrderMaterialEvent.LastUpdated.Date, deserializedResult.LastUpdated.Date);
    }

    [TestMethod]
    public async Task CreateWorkOrderMessage_ValidMessage_ReturnsSerializedWorkOrderMessage()
    {
        // Arrange
        const string message = "1234";
        var query = WorkOrderQuery.GetQuery(long.Parse(message));
        var workOrderEvent = new WorkOrderEvent
        {
            Plant = "ThePlant",
            ProCoSysGuid = Guid.NewGuid(),
            ProjectName = "TheEnigmaticProject",
            WoNo = "101-Dalmatians",
            WoId = 12345,
            CommPkgNo = "CommPkgNo",
            CommPkgGuid = Guid.NewGuid(),
            Title = "Title",
            Description = "Description",
            MilestoneCode = "MilestoneCode",
            SubMilestoneCode = "SubMilestoneCode",
            MilestoneDescription = "MilestoneDescription",
            CategoryCode = "CategoryCode",
            MaterialStatusCode = "MaterialStatusCode",
            HoldByCode = "HoldByCode",
            DisciplineCode = "DisciplineCode",
            DisciplineDescription = "DisciplineDescription",
            ResponsibleCode = "ResponsibleCode",
            ResponsibleDescription = "ResponsibleDescription",
            AreaCode = "AreaCode",
            AreaDescription = "AreaDescription",
            JobStatusCode = "JobStatusCode",
            MaterialComments = "MaterialComments",
            ConstructionComments = "ConstructionComments",
            TypeOfWorkCode = "TypeOfWorkCode",
            OnShoreOffShoreCode = "OnShoreOffShoreCode",
            WoTypeCode = "WoTypeCode",
            ProjectProgress = 3.2,
            ExpendedManHours = "ExpendedManHours",
            EstimatedHours = "EstimatedHours",
            RemainingHours = "RemainingHours",
            WBS = "WBS",
            Progress = 50,
            PlannedStartAtDate = DateOnly.FromDateTime(DateTime.Now.AddDays(10)),
            ActualStartAtDate = DateOnly.FromDateTime(DateTime.Now.AddDays(5)),
            PlannedFinishedAtDate = DateOnly.FromDateTime(DateTime.Now.AddDays(20)),
            ActualFinishedAtDate = DateOnly.FromDateTime(DateTime.Now.AddDays(15)),
            CreatedAt = DateTime.Now,
            IsVoided = false,
            LastUpdated = DateTime.Now
        };

        _dapperRepositoryMock.Setup(repo => repo.QuerySingle<WorkOrderEvent>(
                It.Is<(string, DynamicParameters)>( qs => qs.Item1 == query.queryString),
                message))
            .ReturnsAsync(workOrderEvent);

        // Act
        var result = await _dut.CreateWorkOrderMessage(message);

        // Assert
        Assert.IsNotNull(result);
        var deserializedResult =
            JsonSerializer.Deserialize<WorkOrderEvent>(result, DefaultSerializerHelper.SerializerOptions);

        // Check if the properties are equal
        Assert.AreEqual(workOrderEvent.Plant, deserializedResult.Plant);
        Assert.AreEqual(workOrderEvent.ProCoSysGuid, deserializedResult.ProCoSysGuid);
        Assert.AreEqual(workOrderEvent.ProjectName, deserializedResult.ProjectName);
        Assert.AreEqual(workOrderEvent.WoNo, deserializedResult.WoNo);
        Assert.AreEqual(workOrderEvent.WoId, deserializedResult.WoId);
        Assert.AreEqual(workOrderEvent.CommPkgNo, deserializedResult.CommPkgNo);
        Assert.AreEqual(workOrderEvent.CommPkgGuid, deserializedResult.CommPkgGuid);
        Assert.AreEqual(workOrderEvent.Title, deserializedResult.Title);
        Assert.AreEqual(workOrderEvent.Description, deserializedResult.Description);
        Assert.AreEqual(workOrderEvent.MilestoneCode, deserializedResult.MilestoneCode);
        Assert.AreEqual(workOrderEvent.SubMilestoneCode, deserializedResult.SubMilestoneCode);
        Assert.AreEqual(workOrderEvent.MilestoneDescription, deserializedResult.MilestoneDescription);
        Assert.AreEqual(workOrderEvent.CategoryCode, deserializedResult.CategoryCode);
        Assert.AreEqual(workOrderEvent.MaterialStatusCode, deserializedResult.MaterialStatusCode);
        Assert.AreEqual(workOrderEvent.HoldByCode, deserializedResult.HoldByCode);
        Assert.AreEqual(workOrderEvent.DisciplineCode, deserializedResult.DisciplineCode);
        Assert.AreEqual(workOrderEvent.DisciplineDescription, deserializedResult.DisciplineDescription);
        Assert.AreEqual(workOrderEvent.ResponsibleCode, deserializedResult.ResponsibleCode);
        Assert.AreEqual(workOrderEvent.ResponsibleDescription, deserializedResult.ResponsibleDescription);
        Assert.AreEqual(workOrderEvent.AreaCode, deserializedResult.AreaCode);
        Assert.AreEqual(workOrderEvent.AreaDescription, deserializedResult.AreaDescription);
        Assert.AreEqual(workOrderEvent.JobStatusCode, deserializedResult.JobStatusCode);
        Assert.AreEqual(workOrderEvent.MaterialComments, deserializedResult.MaterialComments);
        Assert.AreEqual(workOrderEvent.ConstructionComments, deserializedResult.ConstructionComments);
        Assert.AreEqual(workOrderEvent.TypeOfWorkCode, deserializedResult.TypeOfWorkCode);
        Assert.AreEqual(workOrderEvent.OnShoreOffShoreCode, deserializedResult.OnShoreOffShoreCode);
        Assert.AreEqual(workOrderEvent.WoTypeCode, deserializedResult.WoTypeCode);
        Assert.AreEqual(workOrderEvent.ProjectProgress, deserializedResult.ProjectProgress);
        Assert.AreEqual(workOrderEvent.ExpendedManHours, deserializedResult.ExpendedManHours);
        Assert.AreEqual(workOrderEvent.EstimatedHours, deserializedResult.EstimatedHours);
        Assert.AreEqual(workOrderEvent.RemainingHours, deserializedResult.RemainingHours);
        Assert.AreEqual(workOrderEvent.WBS, deserializedResult.WBS);
        Assert.AreEqual(workOrderEvent.Progress, deserializedResult.Progress);
        Assert.AreEqual(workOrderEvent.PlannedStartAtDate, deserializedResult.PlannedStartAtDate);
        Assert.AreEqual(workOrderEvent.ActualStartAtDate, deserializedResult.ActualStartAtDate);
        Assert.AreEqual(workOrderEvent.PlannedFinishedAtDate, deserializedResult.PlannedFinishedAtDate);
        Assert.AreEqual(workOrderEvent.ActualFinishedAtDate, deserializedResult.ActualFinishedAtDate);
        Assert.AreEqual(workOrderEvent.CreatedAt.Date, deserializedResult.CreatedAt.Date);
        Assert.AreEqual(workOrderEvent.IsVoided, deserializedResult.IsVoided);
        Assert.AreEqual(workOrderEvent.LastUpdated.Date, deserializedResult.LastUpdated.Date);
    }

    [TestMethod]
    public async Task CreateWorkOrderMilestoneMessage_ValidMessage_ReturnsSerializedWorkOrderMilestoneMessage()
    {
        // Arrange
        const string message = "1234,4321";
        const long workOrderId = 1234L;
        const long milestoneId = 4321L;
        var query = WorkOrderMilestoneQuery.GetQuery(workOrderId, milestoneId);
        var workOrderMilestoneEvent = new WorkOrderMilestoneEvent
        {
            Plant = "ThePlant",
            ProCoSysGuid = Guid.NewGuid(),
            ProjectName = "TheEnigmaticProject",
            WoId = 12345,
            WoGuid = Guid.NewGuid(),
            WoNo = "WONO",
            Code = "CODE",
            MilestoneDate = DateOnly.FromDateTime(DateTime.Now.AddDays(10)),
            SignedByAzureOid = "12345678",
            LastUpdated = DateTime.Now
        };

        _dapperRepositoryMock.Setup(repo => repo.QuerySingle<WorkOrderMilestoneEvent>(
                It.Is<(string, DynamicParameters)>( qs => qs.Item1 == query.queryString),
                message))
            .ReturnsAsync(workOrderMilestoneEvent);

        // Act
        var result = await _dut.CreateWorkOrderMilestoneMessage(message);

        // Assert
        Assert.IsNotNull(result);
        var deserializedResult =
            JsonSerializer.Deserialize<WorkOrderMilestoneEvent>(result, DefaultSerializerHelper.SerializerOptions);

        // Check if the properties are equal
        Assert.AreEqual(workOrderMilestoneEvent.Plant, deserializedResult.Plant);
        Assert.AreEqual(workOrderMilestoneEvent.ProCoSysGuid, deserializedResult.ProCoSysGuid);
        Assert.AreEqual(workOrderMilestoneEvent.ProjectName, deserializedResult.ProjectName);
        Assert.AreEqual(workOrderMilestoneEvent.WoId, deserializedResult.WoId);
        Assert.AreEqual(workOrderMilestoneEvent.WoGuid, deserializedResult.WoGuid);
        Assert.AreEqual(workOrderMilestoneEvent.WoNo, deserializedResult.WoNo);
        Assert.AreEqual(workOrderMilestoneEvent.Code, deserializedResult.Code);
        Assert.AreEqual(workOrderMilestoneEvent.MilestoneDate, deserializedResult.MilestoneDate);
        Assert.AreEqual(workOrderMilestoneEvent.SignedByAzureOid, deserializedResult.SignedByAzureOid);
        Assert.AreEqual(workOrderMilestoneEvent.LastUpdated.Date, deserializedResult.LastUpdated.Date);
    }

    [TestMethod]
    public async Task HandleBusEvents_AddsTagDetailsToTag()
    {
        //Arrange
        const string tagDetailsString = "HubbaBubba";
        _tagDetailsRepositoryMock.Setup(t => t.GetDetailsStringByTagId(116866654))
            .ReturnsAsync(tagDetailsString);
        const string jsonMessage =
            @"{
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
            }";

        //Act
        var result = await _dut.AttachTagDetails(jsonMessage);

        //Assert
        var deserializedTagTopic = JsonSerializer.Deserialize<TagTopic>(result);
        Assert.IsNotNull(deserializedTagTopic);
        Assert.AreEqual(tagDetailsString, deserializedTagTopic.TagDetails);
        Assert.IsNotNull(deserializedTagTopic.McPkgGuid);
        Assert.IsNotNull(deserializedTagTopic.CommPkgGuid);
    }

    [TestInitialize]
    public void Setup()
    {
        _tagDetailsRepositoryMock = new Mock<ITagDetailsRepository>();
        _dapperRepositoryMock = new Mock<IEventRepository>();
        _dut = new BusEventService(_tagDetailsRepositoryMock.Object,
            _dapperRepositoryMock.Object);
    }
}