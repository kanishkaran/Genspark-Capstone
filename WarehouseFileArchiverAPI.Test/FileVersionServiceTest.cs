using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using WarehouseFileArchiverAPI.Interfaces;
using WarehouseFileArchiverAPI.Models;
using WarehouseFileArchiverAPI.Services;

namespace WarehouseFileArchiverAPI.Test
{
    [TestFixture]
    public class FileVersionServiceTest
    {
        private Mock<IRepository<Guid, FileVersion>> _fileVersionRepoMock;
        private FileVersionService _service;

        [SetUp]
        public void Setup()
        {
            _fileVersionRepoMock = new Mock<IRepository<Guid, FileVersion>>();
            _service = new FileVersionService(_fileVersionRepoMock.Object);
        }

        [Test]
        public async Task GetFileVersionByArchiveId_ReturnsLatestVersion()
        {
            var archiveId = Guid.NewGuid();
            var versions = new List<FileVersion>
            {
                new FileVersion { FileArchiveId = archiveId, VersionNumber = 1 },
                new FileVersion { FileArchiveId = archiveId, VersionNumber = 2 }
            };
            _fileVersionRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(versions);

            var result = await _service.GetFileVersionByArchiveId(archiveId);

            Assert.That(result.VersionNumber, Is.EqualTo(2));
        }

        [Test]
        public async Task GetFileVersionByVersionNumber_ReturnsCorrectVersion()
        {
            var archiveId = Guid.NewGuid();
            var versions = new List<FileVersion>
            {
                new FileVersion { FileArchiveId = archiveId, VersionNumber = 1 },
                new FileVersion { FileArchiveId = archiveId, VersionNumber = 2 }
            };
            _fileVersionRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(versions);

            var result = await _service.GetFileVersionByVersionNumber(archiveId, 1);

            Assert.That(result.VersionNumber, Is.EqualTo(1));
        }

        [Test]
        public async Task ValidateChecksum_ReturnsTrue_WhenNoDuplicate()
        {
            var archiveId = Guid.NewGuid();
            var versions = new List<FileVersion>
            {
                new FileVersion { FileArchiveId = archiveId, VersionNumber = 1, Checksum = "abc" }
            };
            _fileVersionRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(versions);

            var result = await _service.ValidateChecksum("def", archiveId);

            Assert.That(result, Is.True);
        }

        [Test]
        public async Task ValidateChecksum_ReturnsFalse_WhenDuplicateExists()
        {
            var archiveId = Guid.NewGuid();
            var versions = new List<FileVersion>
            {
                new FileVersion { FileArchiveId = archiveId, VersionNumber = 1, Checksum = "abc" }
            };
            _fileVersionRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(versions);

            var result = await _service.ValidateChecksum("abc", archiveId);

            Assert.That(result, Is.False);
        }
    }
}