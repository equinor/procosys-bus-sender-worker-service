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
    public async Task CreateCallOffMessage_ValidMessage_ReturnsSerializedCallOffEvent()
    {
        // Arrange
        const string message = "123";
        const long callOffId = 123L;
        var queryString = CallOffQuery.GetQuery(callOffId);
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
        
        _dapperRepositoryMock.Setup(repo => repo.QuerySingle<CallOffEvent>(queryString, message))
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
            FormPhase = "Phase1",
            SystemModule = "Module1",
            FormularDiscipline = "Discipline1",
            Revision = "Revision1",
            PipingRevisionMcPkNo = "PRMP-123",
            PipingRevisionMcPkGuid = Guid.NewGuid(),
            Responsible = "Responsible1",
            Status = "Status1",
            UpdatedAt = DateTime.UtcNow,
            LastUpdated = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            SignedAt = DateTime.UtcNow,
            VerifiedAt = DateTime.UtcNow
        };

        _dapperRepositoryMock.Setup(repo => repo.QuerySingle<ChecklistEvent>(queryString, message))
            .ReturnsAsync(checklistEvent);
        
        // Act
        var result = await _dut.CreateChecklistMessage(message);
        
        // Assert
        Assert.IsNotNull(result);
        var deserializedResult = JsonSerializer.Deserialize<ChecklistEvent>(result, DefaultSerializerHelper.SerializerOptions);
        
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
        Assert.AreEqual(checklistEvent.FormPhase, deserializedResult.FormPhase);
        Assert.AreEqual(checklistEvent.SystemModule, deserializedResult.SystemModule);
        Assert.AreEqual(checklistEvent.FormularDiscipline, deserializedResult.FormularDiscipline);
        Assert.AreEqual(checklistEvent.Revision, deserializedResult.Revision);
        Assert.AreEqual(checklistEvent.PipingRevisionMcPkNo, deserializedResult.PipingRevisionMcPkNo);
        Assert.AreEqual(checklistEvent.PipingRevisionMcPkGuid, deserializedResult.PipingRevisionMcPkGuid);
        Assert.AreEqual(checklistEvent.Responsible, deserializedResult.Responsible);
        Assert.AreEqual(checklistEvent.Status, deserializedResult.Status);
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
        
        _dapperRepositoryMock.Setup(repo => repo.QuerySingle<CommPkgEvent>(queryString, message))
            .ReturnsAsync(commPkgEvent);
        
        // Act
        var result = await _dut.CreateCommPkgMessage(message);
        
        // Assert
        Assert.IsNotNull(result);
        var deserializedResult = JsonSerializer.Deserialize<CommPkgEvent>(result, DefaultSerializerHelper.SerializerOptions);
        
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
        var queryString = CommPkgMilestoneQuery.GetQuery(commPkgGuid);
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

        _dapperRepositoryMock.Setup(repo => repo.QuerySingle<CommPkgMilestoneEvent>(queryString, message))
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
        var queryString = CommPkgOperationQuery.GetQuery(commPkgId);
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

        _dapperRepositoryMock.Setup(repo => repo.QuerySingle<CommPkgOperationEvent>(queryString, message))
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
        var queryString = QueryCommPkgQuery.GetQuery(commPkgId,documentId);
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


        _dapperRepositoryMock.Setup(repo => repo.QuerySingle<CommPkgQueryEvent>(queryString, message))
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
        var queryString = CommPkgTaskQuery.GetQuery(commPkgId,taskId);
        
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


        _dapperRepositoryMock.Setup(repo => repo.QuerySingle<CommPkgTaskEvent>(queryString, message))
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
        var queryString = HeatTraceQuery.GetQuery(heatTraceId);
        
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

        
        _dapperRepositoryMock.Setup(repo => repo.QuerySingle<HeatTraceEvent>(queryString, message))
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
        var queryString = LibraryFieldQuery.GetQuery(libraryFieldGuid);
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

        _dapperRepositoryMock.Setup(repo => repo.QuerySingle<LibraryFieldEvent>(queryString, message))
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

    public async Task CreateLibraryMessage_ValidMessage_ReturnsSerializedLibraryEvent()
    {
        //Arrange
        const string message = "123";
        // Act
        var result = await _dut.CreateLibraryMessage(message);
    }
    

    [TestMethod]
    public async Task CreateMcPkgMilestoneMessage_ValidMessage_ReturnsSerializedMcPkgMilestoneEvent()
    {
        // Arrange
        const string message = "5f643eeb-b114-4fc4-b884-ade3f6ea63ce";;
        const string mcPkgGuid = "5f643eeb-b114-4fc4-b884-ade3f6ea63ce";;
        var queryString = McPkgMilestoneQuery.GetQuery(mcPkgGuid);
        
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
        
        _dapperRepositoryMock.Setup(repo => repo.QuerySingle<McPkgMilestoneEvent>(queryString, message))
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
    public async Task CreateSwcrMessage_ValidMessage_ReturnsSerializedSwcrEvent()
    {
        // Arrange
        const string message = "12345";
        const long swcrId = 12345L;
        var queryString = SwcrQuery.GetQuery(swcrId);
        var swcrEvent = new SwcrEvent
        {
            Plant = "Plant1",
            ProCoSysGuid = Guid.NewGuid(),
            ProjectName = "ProjectName1",
            SWCRNO = "SWCRNO1",
            SWCRId = 1,
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


        
        _dapperRepositoryMock.Setup(repo => repo.QuerySingle<SwcrEvent>(queryString, message))
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
        Assert.AreEqual(swcrEvent.SWCRNO, deserializedResult.SWCRNO);
        Assert.AreEqual(swcrEvent.SWCRId, deserializedResult.SWCRId);
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