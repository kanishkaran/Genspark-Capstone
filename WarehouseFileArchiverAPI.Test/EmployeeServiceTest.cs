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
   
    public class EmployeeServiceTest
    {
        private Mock<IRepository<string, User>> _userRepoMock;
        private Mock<IRepository<Guid, Employee>> _employeeRepoMock;
        private Mock<IEncryptionService> _encryptionServiceMock;
        private Mock<IRepository<Guid, Role>> _roleRepoMock;
        private Mock<IAuditLogService> _auditLogServiceMock;
        private EmployeeService _service;

        [SetUp]
        public void Setup()
        {
            _userRepoMock = new Mock<IRepository<string, User>>();
            _employeeRepoMock = new Mock<IRepository<Guid, Employee>>();
            _encryptionServiceMock = new Mock<IEncryptionService>();
            _roleRepoMock = new Mock<IRepository<Guid, Role>>();
            _auditLogServiceMock = new Mock<IAuditLogService>();
            _service = new EmployeeService(
                _userRepoMock.Object,
                _employeeRepoMock.Object,
                _encryptionServiceMock.Object,
                _roleRepoMock.Object,
                _auditLogServiceMock.Object
            );
        }

        [Test]
        public async Task GetEmployeeByEmail_ReturnsEmployee_WhenActive()
        {
            var employee = new Employee { Email = "test@email.com", IsActive = true };
            _employeeRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Employee> { employee });

            var result = await _service.GetEmployeeByEmail("test@email.com");

            Assert.That(result.Email, Is.EqualTo("test@email.com"));
            Assert.That(result.IsActive, Is.True);
        }

        [Test]
        public void GetEmployeeByEmail_Throws_WhenNotFound()
        {
            _employeeRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Employee>());

            Assert.ThrowsAsync<CollectionEmptyException>(() => _service.GetEmployeeByEmail("nouser@email.com"));
        }

        [Test]
        public async Task DeleteEmployee_MarksInactiveAndLogs()
        {
            var id = Guid.NewGuid();
            var employee = new Employee { Id = id, Email = "emp@email.com", IsActive = true };
            _employeeRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(employee);
            _employeeRepoMock.Setup(r => r.UpdateAsync(id, It.IsAny<Employee>())).ReturnsAsync(employee);

            var result = await _service.DeleteEmployee(id, "admin@email.com");

            Assert.That(result, Does.Contain(id.ToString()));
            _employeeRepoMock.Verify(r => r.UpdateAsync(id, It.Is<Employee>(e => !e.IsActive)), Times.Once);
            _auditLogServiceMock.Verify(a => a.LogAsync(
                "Employee", id, "Delete", "admin@email.com", It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void DeleteEmployee_Throws_WhenNotFoundOrInactive()
        {
            var id = Guid.NewGuid();
            _employeeRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Employee)null);

            Assert.ThrowsAsync<EmployeeNotFoundException>(() => _service.DeleteEmployee(id, "admin@email.com"));
        }
    }
}