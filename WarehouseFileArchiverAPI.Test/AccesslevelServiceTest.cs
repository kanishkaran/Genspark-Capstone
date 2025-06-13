using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using WarehouseFileArchiverAPI.Interfaces;
using WarehouseFileArchiverAPI.Models;
using WarehouseFileArchiverAPI.Models.DTOs;
using WarehouseFileArchiverAPI.Services;
using WarehouseFileArchiverAPI.Exceptions;

namespace WarehouseFileArchiverAPI.Test
{

    public class AccessLevelServiceTest
    {
        private Mock<IRepository<Guid, AccessLevel>> _accessLevelRepoMock;
        private Mock<IAuditLogService> _auditLogServiceMock;
        private AccessLevelService _service;

        [SetUp]
        public void Setup()
        {
            _accessLevelRepoMock = new Mock<IRepository<Guid, AccessLevel>>();
            _auditLogServiceMock = new Mock<IAuditLogService>();
            _service = new AccessLevelService(_accessLevelRepoMock.Object, _auditLogServiceMock.Object);
        }

        [Test]
        public async Task GetAccessLevelById_ReturnsAccessLevel_WhenActive()
        {
            var id = Guid.NewGuid();
            var accessLevel = new AccessLevel { Id = id, Access = "Read", IsActive = true };
            _accessLevelRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(accessLevel);

            var result = await _service.GetAccessLevelById(id);

            Assert.That(result.Id, Is.EqualTo(id));
            Assert.That(result.Access, Is.EqualTo("Read"));
            Assert.That(result.IsActive, Is.True);
        }

        [Test]
        public void GetAccessLevelById_Throws_WhenNotFoundOrInactive()
        {
            var id = Guid.NewGuid();
            _accessLevelRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((AccessLevel)null);

            Assert.ThrowsAsync<InvalidAccessLevelException>(() => _service.GetAccessLevelById(id));
        }

        [Test]
        public async Task DeleteAccessLevel_MarksInactiveAndLogs()
        {
            var id = Guid.NewGuid();
            var accessLevel = new AccessLevel { Id = id, Access = "Write", IsActive = true };
            _accessLevelRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(accessLevel);
            _accessLevelRepoMock.Setup(r => r.UpdateAsync(id, It.IsAny<AccessLevel>())).ReturnsAsync(accessLevel);

            var result = await _service.DeleteAccessLevel(id, "admin");

            Assert.That(result, Does.Contain(id.ToString()));
            _accessLevelRepoMock.Verify(r => r.UpdateAsync(id, It.Is<AccessLevel>(a => !a.IsActive)), Times.Once);
            _auditLogServiceMock.Verify(a => a.LogAsync(
                "AccessLevel", id, "Delete", "admin", It.IsAny<string>()), Times.Once);
        }
    }
}