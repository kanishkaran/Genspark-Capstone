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
    public class CategoryServiceTest
    {
        private Mock<IRepository<Guid, Category>> _categoryRepoMock;
        private Mock<IRepository<Guid, AccessLevel>> _accessLevelRepoMock;
        private Mock<IAuditLogService> _auditLogServiceMock;
        private CategoryService _service;

        [SetUp]
        public void Setup()
        {
            _categoryRepoMock = new Mock<IRepository<Guid, Category>>();
            _accessLevelRepoMock = new Mock<IRepository<Guid, AccessLevel>>();
            _auditLogServiceMock = new Mock<IAuditLogService>();
            _service = new CategoryService(_categoryRepoMock.Object, _accessLevelRepoMock.Object, _auditLogServiceMock.Object);
        }

        [Test]
        public async Task AddCategory_AddsAndLogs()
        {
            var accessLevel = new AccessLevel { Id = Guid.NewGuid(), Access = "Admin" };
            var dto = new CategoryAddRequestDto { CategoryName = "Legal", AccessLevel = "Admin" };
            _accessLevelRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<AccessLevel> { accessLevel });
            _categoryRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Category>());
            _categoryRepoMock.Setup(r => r.AddAsync(It.IsAny<Category>()))
                .ReturnsAsync((Category c) => c);

            var result = await _service.AddCategory(dto, "testuser");

            Assert.That(result.CategoryName, Is.EqualTo("Legal"));
            Assert.That(result.AccessLevelId, Is.EqualTo(accessLevel.Id));
            _auditLogServiceMock.Verify(a => a.LogAsync(
                "Category", It.IsAny<Guid>(), "Add", "testuser", It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void AddCategory_Throws_WhenInvalidAccessLevel()
        {
            var dto = new CategoryAddRequestDto { CategoryName = "Finance", AccessLevel = "Invalid" };
            _accessLevelRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<AccessLevel>());

            Assert.ThrowsAsync<InvalidAccessLevelException>(() => _service.AddCategory(dto, "testuser"));
        }

        [Test]
        public async Task GetById_ReturnsCategory()
        {
            var id = Guid.NewGuid();
            var category = new Category { Id = id, CategoryName = "Finance", IsDeleted = false };
            _categoryRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(category);

            var result = await _service.GetById(id);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.CategoryName, Is.EqualTo("Finance"));
        }

        [Test]
        public void GetById_Throws_WhenNotFound()
        {
            var id = Guid.NewGuid();
            _categoryRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Category?)null);

            Assert.ThrowsAsync<CategoryNotFoundException>(() => _service.GetById(id));
        }
    }
}