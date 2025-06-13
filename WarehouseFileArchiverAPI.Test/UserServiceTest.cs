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
using Microsoft.Extensions.Configuration;

namespace WarehouseFileArchiverAPI.Test
{

    public class UserServiceTest
    {
        private Mock<IRepository<string, User>> _userRepoMock;
        private Mock<IRepository<Guid, Role>> _roleRepoMock;
        private Mock<IEncryptionService> _encryptionServiceMock;
        private Mock<IAuditLogService> _auditLogServiceMock;
        private Mock<IConfiguration> _configMock;
        private UserService _service;

        [SetUp]
        public void Setup()
        {
            _userRepoMock = new Mock<IRepository<string, User>>();
            _roleRepoMock = new Mock<IRepository<Guid, Role>>();
            _encryptionServiceMock = new Mock<IEncryptionService>();
            _auditLogServiceMock = new Mock<IAuditLogService>();
            _configMock = new Mock<IConfiguration>();
            _service = new UserService(
                _userRepoMock.Object,
                _roleRepoMock.Object,
                _encryptionServiceMock.Object,
                _configMock.Object,
                _auditLogServiceMock.Object
            );
        }

        [Test]
        public async Task GetById_ReturnsUserListDto_WhenUserExists()
        {
            var user = new User { Username = "testuser", RoleId = Guid.NewGuid(), IsDeleted = false };
            var role = new Role { Id = user.RoleId, RoleName = "User" };
            _userRepoMock.Setup(r => r.GetByIdAsync("testuser")).ReturnsAsync(user);
            _roleRepoMock.Setup(r => r.GetByIdAsync(user.RoleId)).ReturnsAsync(role);

            var result = await _service.GetById("testuser");

            Assert.That(result.Username, Is.EqualTo("testuser"));
            Assert.That(result.RoleName, Is.EqualTo("User"));
            Assert.That(result.IsDeleted, Is.False);
        }

        [Test]
        public void GetById_WhenUserNotFound()
        {
            _userRepoMock.Setup(r => r.GetByIdAsync("nouser")).ReturnsAsync((User)null);

            Assert.ThrowsAsync<CollectionEmptyException>(() => _service.GetById("nouser"));
        }

        [Test]
        public async Task DeleteUser()
        {
            var user = new User { Username = "deluser", IsDeleted = false };
            _userRepoMock.Setup(r => r.GetByIdAsync("deluser")).ReturnsAsync(user);
            _userRepoMock.Setup(r => r.UpdateAsync("deluser", It.IsAny<User>())).ReturnsAsync(user);

            var result = await _service.DeleteUser("deluser", "admin");

            Assert.That(result, Does.Contain("deluser"));
            _userRepoMock.Verify(r => r.UpdateAsync("deluser", It.Is<User>(u => u.IsDeleted)), Times.Once);
        }

        [Test]
        public void DeleteUser_Throws_AlreadyDeleted()
        {
            var user = new User { Username = "deluser", IsDeleted = true };
            _userRepoMock.Setup(r => r.GetByIdAsync("deluser")).ReturnsAsync(user);

            Assert.ThrowsAsync<CollectionEmptyException>(() => _service.DeleteUser("deluser", "admin"));
        }
    }
}