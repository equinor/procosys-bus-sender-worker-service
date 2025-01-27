using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Equinor.ProCoSys.BusSenderWorker.Core.Interfaces;
using Equinor.ProCoSys.BusSenderWorker.Core.Services;
using Equinor.ProCoSys.BusSenderWorker.Core.Telemetry;
using Equinor.ProCoSys.PcsServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.BusSenderWorker.Core.Tests;

[TestClass]
public class QueueMonitorServiceTests
{
    private Mock<ITelemetryClient> _mockTelemetryClient;
    private Mock<IBusEventRepository> _mockBusEventRepository;
    private IOptions<InstanceOptions> _instanceOptions;
    private ManualTimeProvider _manualTimeProvider;
    private QueueMonitorService _dut;
    private IConfiguration _configuration;

    [TestInitialize]
    public void Initialize()
    {
        _mockTelemetryClient = new Mock<ITelemetryClient>();
        _mockBusEventRepository = new Mock<IBusEventRepository>();
        _manualTimeProvider = new ManualTimeProvider();
        _instanceOptions = Options.Create(new InstanceOptions { InstanceName = "TestInstance" });

        var inMemorySettings = new Dictionary<string, string> {
            {"QueueWriteIntervalMinutes", "15"},
        };
        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();
        
        _dut = new QueueMonitorService(
            _mockTelemetryClient.Object,
            _mockBusEventRepository.Object,
            _configuration,
            _manualTimeProvider,
            _instanceOptions);
    }

    [TestMethod]
    public async Task WriteQueueMetrics_ShouldWriteQueueLengthAndAge()
    {
        // Arrange
        //TimeService.SetProvider(new ManualTimeProvider(new DateTime(2020, 1, 1, 12, 0, 0, DateTimeKind.Utc)));
        _manualTimeProvider.Set(new DateTime(2020, 1, 1, 12, 0, 0, DateTimeKind.Utc));
        var fakeDateTime = _manualTimeProvider.UtcNow;
        _mockBusEventRepository
            .Setup(m => m.GetUnProcessedCount())
            .ReturnsAsync(10); 
        _mockBusEventRepository
            .Setup(m => m.GetOldestEvent())
            .ReturnsAsync(fakeDateTime.AddMinutes(-1470));

        // Act
        await _dut.WriteQueueMetrics();

        // Assert
        var properties = new Dictionary<string, string>
        {
            { "InstanceName", "TestInstance" }
        };
        _mockTelemetryClient.Verify(t => t.TrackMetric("QueueLength", 10, properties), Times.Once);
        _mockTelemetryClient.Verify(t => t.TrackMetric("QueueAge", 1470, properties), Times.Once);
    }

    [TestMethod]
    public async Task WriteQueueMetrics_WhenElapsedIsGreaterThanSpecified_ShouldWriteQueueLengthAndAge()
    {
        // Arrange
        var initialDateTime = new DateTime(2020, 1, 1, 12, 10, 0, DateTimeKind.Utc);
        _manualTimeProvider.Set(initialDateTime); 

        // Act
        await _dut.WriteQueueMetrics();
        _manualTimeProvider.Set(initialDateTime.AddMinutes(50)); 
        await _dut.WriteQueueMetrics();

        // Assert
        var properties = new Dictionary<string, string>
        {
            { "InstanceName", "TestInstance" }
        };
        _mockTelemetryClient.Verify(t => t.TrackMetric("QueueAge", It.IsAny<double>(), properties), Times.Exactly(2));
    }

    [TestMethod]
    public async Task WriteQueueMetrics_WhenElapsedIsLessThanSpecified_ShouldNotWriteQueueLengthAndAge()
    {
        // Arrange
        _manualTimeProvider.Set(new DateTime(2020, 1, 1, 12, 10, 0, DateTimeKind.Utc)); 

        // Act
        await _dut.WriteQueueMetrics();//Always write the first time
        await _dut.WriteQueueMetrics();

        // Assert
        var properties = new Dictionary<string, string>
        {
            { "InstanceName", "TestInstance" }
        };
        _mockTelemetryClient.Verify(t => t.TrackMetric("QueueAge", It.IsAny<double>(), properties), Times.Once);
    }

    [TestMethod]
    public async Task WriteQueueMetrics_IfNoEventsFound_ShouldWriteQueueAgeAsZero()
    {
        // Arrange
        _manualTimeProvider.Set(new DateTime(2020, 1, 1, 12, 10, 0, DateTimeKind.Utc));

        _mockBusEventRepository.Setup(m => m.GetOldestEvent())
            .ReturnsAsync(DateTime.MinValue); // Mock no events found

        // Act
        await _dut.WriteQueueMetrics();

        // Assert
        var properties = new Dictionary<string, string>
        {
            { "InstanceName", "TestInstance" }
        };
        _mockTelemetryClient.Verify(t => t.TrackMetric("QueueAge", 0, properties), Times.Once); 
    }
}