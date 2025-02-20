using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Equinor.ProCoSys.BusSenderWorker.Core.Models;
using Equinor.ProCoSys.BusSenderWorker.Core.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.BusSenderWorker.Core.Tests
{
    [TestClass]
    public class BlobLeaseServiceTests
    {
        private Mock<ILogger<BlobLeaseService>> _loggerMock;
        private Mock<IConfiguration> _configurationMock;
        private Mock<IMemoryCache> _cacheMock;
        private Mock<BlobLeaseClient> _blobLeaseClientMock;
        private Mock<BlobClient> _blobClientMock;
        private Mock<BlobLeaseService> _blobLeaseServiceMock;

        [TestInitialize]
        public void Setup()
        {
            _loggerMock = new Mock<ILogger<BlobLeaseService>>();
            _configurationMock = new Mock<IConfiguration>();
            _cacheMock = new Mock<IMemoryCache>();
            _blobClientMock = new Mock<BlobClient>();
            _blobLeaseClientMock = new Mock<BlobLeaseClient>();

            _blobLeaseServiceMock = new Mock<BlobLeaseService>(_loggerMock.Object, _configurationMock.Object, _cacheMock.Object) { CallBase = true }; // Partial mock.

            _configurationMock.SetupGet(x => x["MaxBlobReleaseLeaseAttempts"]).Returns("3");
            _configurationMock.SetupGet(x => x["BlobLeaseExpiryTime"]).Returns("60");
            _configurationMock.SetupGet(x => x["BlobReleaseLeaseDelay"]).Returns("2");

            _blobLeaseServiceMock
                .Setup(x => x.GetBlobClient())
                .Returns(_blobClientMock.Object);

            _blobLeaseServiceMock
                .Setup(x => x.GetBlobLeaseClient(It.IsAny<BlobClient>(), It.IsAny<string>()))
                .Returns(_blobLeaseClientMock.Object);

            _blobLeaseClientMock.Setup(x => x.AcquireAsync(It.IsAny<TimeSpan>(), It.IsAny<RequestConditions>(), It.IsAny<CancellationToken>())).ReturnsAsync(Response.FromValue(BlobsModelFactory.BlobLease(new ETag(), DateTimeOffset.UtcNow, "leaseId"), null));
            _blobLeaseClientMock.Setup(x => x.ReleaseAsync(It.IsAny<RequestConditions>(), It.IsAny<CancellationToken>())).ReturnsAsync(Response.FromValue(new ReleasedObjectInfo(new ETag(), DateTimeOffset.UtcNow), null));

            _blobClientMock.Setup(x => x.DownloadAsync()).ReturnsAsync(Response.FromValue(BlobsModelFactory.BlobDownloadInfo(), null));
            _blobClientMock.Setup(x => x.UploadAsync(It.IsAny<MemoryStream>(), It.IsAny<BlobUploadOptions>(), It.IsAny<CancellationToken>())).ReturnsAsync(Response.FromValue(BlobsModelFactory.BlobContentInfo(new ETag(), DateTimeOffset.UtcNow, Array.Empty<byte>(), null, 0), null));

            object cacheEntry = null;
            _cacheMock.Setup(x => x.TryGetValue(It.IsAny<object>(), out cacheEntry)).Returns(false);
            _cacheMock.Setup(x => x.CreateEntry(It.IsAny<object>())).Returns(Mock.Of<ICacheEntry>());
        }

        [TestMethod]
        public async Task ClaimPlantLease_ShouldReturnPlantLease_WhenAvailableLeaseIsAcquired()
        {
            var jsonString = @"
                    [
                        {
                         ""Plant"":""Plant1"",
                         ""LeaseExpiry"":null,
                         ""LastProcessed"":""2025-01-01T00:00:00""
                        }
                    ]";

            _blobLeaseServiceMock
                .Setup(x => x.GetBlobContentAsync(_blobClientMock.Object)).ReturnsAsync(jsonString);

            // Act
            var result = await _blobLeaseServiceMock.Object.ClaimPlantLease();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
            Assert.IsTrue(result[0].IsCurrent);
        }

        [TestMethod]
        public async Task ClaimPlantLease_ShouldReturnPlantLease_WhenExpiredLease()
        {
            var jsonString = @"
                    [
                        {
                         ""Plant"":""Plant1"",
                         ""LeaseExpiry"":""1900-01-01T00:00:00"",
                         ""LastProcessed"":""2025-01-01T00:00:00""
                        }
                    ]";

            _blobLeaseServiceMock
                .Setup(x => x.GetBlobContentAsync(_blobClientMock.Object)).ReturnsAsync(jsonString);

            // Act
            var result = await _blobLeaseServiceMock.Object.ClaimPlantLease();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
            Assert.IsTrue(result[0].IsCurrent);
        }

        [TestMethod]
        public async Task ClaimPlantLease_ShouldReturnOldestProcessed_WhenMultipleAvailableLeases()
        {
            var jsonString = @"
                    [
                        {
                         ""Plant"":""Plant1"",
                         ""LeaseExpiry"":null,
                         ""LastProcessed"":""2025-01-01T00:00:00""
                        },
                        {
                         ""Plant"":""Plant2"",
                         ""LeaseExpiry"":null,
                         ""LastProcessed"":""2024-01-01T00:00:00""
                        }
                    ]";

            _blobLeaseServiceMock
                .Setup(x => x.GetBlobContentAsync(_blobClientMock.Object)).ReturnsAsync(jsonString);

            // Act
            var result = await _blobLeaseServiceMock.Object.ClaimPlantLease();
            var plantHandledByCurrentInstance = result.First(x => x.IsCurrent);

            // Assert
            Assert.IsNotNull(plantHandledByCurrentInstance);
            Assert.AreEqual("Plant2",plantHandledByCurrentInstance.Plant);
        }


        [TestMethod]
        public async Task ClaimPlantLease_ShouldReturnNull_WhenNoAvailableLeases()
        {
            var jsonString = @"
                    [
                        {
                         ""Plant"":""Plant1"",
                         ""LeaseExpiry"":""2099-01-01T00:00:00"",
                         ""LastProcessed"":""2025-01-01T00:00:00""
                        },
                        {
                         ""Plant"":""Plant2"",
                         ""LeaseExpiry"":""2099-01-01T00:00:00"",
                         ""LastProcessed"":""2024-01-01T00:00:00""
                        }
                    ]";

            _blobLeaseServiceMock
                .Setup(x => x.GetBlobContentAsync(_blobClientMock.Object)).ReturnsAsync(jsonString);

            // Act
            var result = await _blobLeaseServiceMock.Object.ClaimPlantLease();

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task ReleasePlantLease_ShouldSetLeaseExpiryToNull_WhenBlobIsAvailable()
        {
            var jsonString = @"
                    [
                        {
                         ""Plant"":""Plant1"",
                         ""LeaseExpiry"":null,
                         ""LastProcessed"":""2025-01-01T00:00:00""
                        },
                        {
                         ""Plant"":""Plant2"",
                         ""LeaseExpiry"":null,
                         ""LastProcessed"":""2024-01-01T00:00:00""
                        }
                    ]";

            _blobLeaseServiceMock
                .Setup(x => x.GetBlobContentAsync(_blobClientMock.Object)).ReturnsAsync(jsonString);

            // Act
            var result = await _blobLeaseServiceMock.Object.ClaimPlantLease();
            var plantHandledByCurrentInstance = result.First(x => x.IsCurrent);
            _blobLeaseServiceMock.Object.ReleasePlantLease(plantHandledByCurrentInstance);

            _blobLeaseServiceMock.Verify(service => service.UpdatePlantLeases(
                    It.Is<List<PlantLease>>(leases => leases.Any(x => x.Plant == "Plant2" && !x.IsCurrent && x.LeaseExpiry == null)),
                    It.IsAny<string>()),
                Times.Once);
        }


        [TestMethod]
        public void ReleasePlantLease_ShouldKeepLeaseExpiry_WhenBlobIsNotAvailableAfterMaxRetries()
        {
            var jsonString = @"
                    [
                        {
                         ""Plant"":""Plant1"",
                         ""LeaseExpiry"":null,
                         ""LastProcessed"":""2025-01-01T00:00:00""
                        },
                        {
                         ""Plant"":""Plant2"",
                         ""LeaseExpiry"":null,
                         ""LastProcessed"":""2024-01-01T00:00:00""
                        }
                    ]";

            _blobLeaseServiceMock
                .Setup(x => x.GetBlobContentAsync(_blobClientMock.Object)).ReturnsAsync(jsonString);

            // Act
            var plantHandledByCurrentInstance = new PlantLease { Plant = "Plant2", IsCurrent = true, LeaseExpiry = DateTime.UtcNow.AddMinutes(1) };
            _blobLeaseClientMock
                .SetupSequence(x => x.AcquireAsync(It.IsAny<TimeSpan>(), It.IsAny<RequestConditions>(),
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(new RequestFailedException(0, "Lease already present",
                    BlobErrorCode.LeaseAlreadyPresent.ToString(), null))
                .ThrowsAsync(new RequestFailedException(0, "Lease already present",
                    BlobErrorCode.LeaseAlreadyPresent.ToString(), null))
                .ThrowsAsync(new RequestFailedException(0, "Lease already present",
                    BlobErrorCode.LeaseAlreadyPresent.ToString(), null))
                .ThrowsAsync(new RequestFailedException(0, "Lease already present",
                    BlobErrorCode.LeaseAlreadyPresent.ToString(), null));

            _blobLeaseServiceMock.Object.ReleasePlantLease(plantHandledByCurrentInstance);

            _blobLeaseServiceMock.Verify(service => service.UpdatePlantLeases(
                    It.IsAny<List<PlantLease>>(),
                    It.IsAny<string>()),
                Times.Never(), "When lease can not be taken upon release, UpdatePlantLeases should never be called.");
        }
    }
}
