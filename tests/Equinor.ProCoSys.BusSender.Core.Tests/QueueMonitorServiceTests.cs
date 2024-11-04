using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Equinor.ProCoSys.BusSenderWorker.Core.Interfaces;
using Equinor.ProCoSys.BusSenderWorker.Core.Services;
using Equinor.ProCoSys.BusSenderWorker.Core.Telemetry;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.BusSenderWorker.Core.Tests;

[TestClass]
public class QueueMonitorServiceTests
{
    private Mock<ITelemetryClient> _mockTelemetryClient;
    private Mock<IBusEventRepository> _mockBusEventRepository;
    private ManualTimeProvider _manualTimeProvider;
    private QueueMonitorService _dut;
    private IConfiguration _configuration;

    [TestInitialize]
    public void Initialize()
    {
        _mockTelemetryClient = new Mock<ITelemetryClient>();
        _mockBusEventRepository = new Mock<IBusEventRepository>();
        _manualTimeProvider = new ManualTimeProvider();

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
            _manualTimeProvider);
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
        _mockTelemetryClient.Verify(t => t.TrackMetric("QueueLength", 10), Times.Once);
        _mockTelemetryClient.Verify(t => t.TrackMetric("QueueAge", 1470), Times.Once);
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
        _mockTelemetryClient.Verify(t => t.TrackMetric("QueueAge", It.IsAny<double>()), Times.Exactly(2));
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
        _mockTelemetryClient.Verify(t => t.TrackMetric("QueueAge", It.IsAny<double>()), Times.Once);
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
        _mockTelemetryClient.Verify(t => t.TrackMetric("QueueAge", 0), Times.Once); 
    }
}