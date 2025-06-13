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
    [TestFixture]
    public class RoleServiceTest
    {
        private Mock<IRepository<Guid, AccessLevel>> _accessLevelRepoMock;
        private Mock<IRepository<Guid, Role>> _roleRepoMock;
        private Mock<IAuditLogService> _auditLogServiceMock;
        private RoleService _service;

        [SetUp]
        public void Setup()
        {
            _accessLevelRepoMock = new Mock<IRepository<Guid, AccessLevel>>();
            _roleRepoMock = new Mock<IRepository<Guid, Role>>();
            _auditLogServiceMock = new Mock<IAuditLogService>();
            _service = new RoleService(
                _accessLevelRepoMock.Object,
                _roleRepoMock.Object,
                _auditLogServiceMock.Object
            );
        }

        [Test]
        public async Task GetById_ReturnsRole_WhenExistsAndNotDeleted()
        {
            var id = Guid.NewGuid();
            var role = new Role { Id = id, RoleName = "Manager", IsDeleted = false };
            _roleRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(role);

            var result = await _service.GetById(id);

            Assert.That(result.Id, Is.EqualTo(id));
            Assert.That(result.RoleName, Is.EqualTo("Manager"));
            Assert.That(result.IsDeleted, Is.False);
        }

        [Test]
        public void GetById_Throws_WhenNotFoundOrDeleted()
        {
            var id = Guid.NewGuid();
            _roleRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Role)null);

            Assert.ThrowsAsync<Exception>(() => _service.GetById(id));
        }

        [Test]
        public async Task DeleteRole_MarksRoleAsDeletedAndLogs()
        {
            var id = Guid.NewGuid();
            var role = new Role { Id = id, RoleName = "User", IsDeleted = false };
            _roleRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(role);
            _roleRepoMock.Setup(r => r.UpdateAsync(id, It.IsAny<Role>())).ReturnsAsync(role);

            var result = await _service.DeleteRole(id, "admin");

            Assert.That(result, Is.True);
            _roleRepoMock.Verify(r => r.UpdateAsync(id, It.Is<Role>(r => r.IsDeleted)), Times.Once);
            _auditLogServiceMock.Verify(a => a.LogAsync(
                "Role", id, "Delete", "admin", It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void DeleteRole_Throws_WhenNotFoundOrAlreadyDeleted()
        {
            var id = Guid.NewGuid();
            _roleRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Role)null);

            Assert.ThrowsAsync<Exception>(() => _service.DeleteRole(id, "admin"));
        }
    }
}