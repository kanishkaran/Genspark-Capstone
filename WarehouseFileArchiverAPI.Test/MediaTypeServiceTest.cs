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
    public class MediaTypeServiceTest
    {
        private Mock<IRepository<Guid, MediaType>> _mediaTypeRepoMock;
        private Mock<IAuditLogService> _auditLogServiceMock;
        private MediaTypeService _service;

        [SetUp]
        public void Setup()
        {
            _mediaTypeRepoMock = new Mock<IRepository<Guid, MediaType>>();
            _auditLogServiceMock = new Mock<IAuditLogService>();
            _service = new MediaTypeService(_mediaTypeRepoMock.Object, _auditLogServiceMock.Object);
        }

        [Test]
        public async Task GetById_ReturnsMediaType_WhenExists()
        {
            var id = Guid.NewGuid();
            var mediaType = new MediaType { Id = id, TypeName = "image/png", Extension = ".png", IsDeleted = false };
            _mediaTypeRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(mediaType);

            var result = await _service.GetById(id);

            Assert.That(result.Id, Is.EqualTo(id));
            Assert.That(result.TypeName, Is.EqualTo("image/png"));
        }

        [Test]
        public void GetById_Throws_WhenNotFoundOrDeleted()
        {
            var id = Guid.NewGuid();
            _mediaTypeRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((MediaType)null);

            Assert.ThrowsAsync<CollectionEmptyException>(() => _service.GetById(id));
        }

        [Test]
        public async Task AddMediaType_Success()
        {
            var dto = new MediaTypeAddRequestDto { TypeName = "application/pdf", Extension = ".pdf" };
            _mediaTypeRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<MediaType>());
            _mediaTypeRepoMock.Setup(r => r.AddAsync(It.IsAny<MediaType>())).ReturnsAsync(new MediaType { Id = Guid.NewGuid(), TypeName = "application/pdf", Extension = ".pdf" });

            var result = await _service.AddMediaType(dto, "admin");

            Assert.That(result.TypeName, Is.EqualTo("application/pdf"));
            _auditLogServiceMock.Verify(a => a.LogAsync("MediaType", result.Id, "Add", "admin", It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void AddMediaType_Throws_WhenDuplicate()
        {
            var dto = new MediaTypeAddRequestDto { TypeName = "application/pdf", Extension = ".pdf" };
            var existing = new MediaType { TypeName = "application/pdf", Extension = ".pdf", IsDeleted = false };
            _mediaTypeRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<MediaType> { existing });

            Assert.ThrowsAsync<InvalidMediaException>(() => _service.AddMediaType(dto, "admin"));
        }
    }
}