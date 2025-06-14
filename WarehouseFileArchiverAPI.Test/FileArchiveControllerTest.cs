using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using WarehouseFileArchiverAPI.Controllers;
using WarehouseFileArchiverAPI.Interfaces;
using WarehouseFileArchiverAPI.Models.DTOs;

namespace WarehouseFileArchiverAPI.Test
{
    [TestFixture]
    public class FileArchiveControllerTest
    {
        private Mock<IFileArchiveService> _fileArchiveServiceMock;
        private FileArchiveController _controller;

        [SetUp]
        public void SetUp()
        {
            _fileArchiveServiceMock = new Mock<IFileArchiveService>();
            _controller = new FileArchiveController(_fileArchiveServiceMock.Object);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "testuser@email.com"),
                new Claim(ClaimTypes.Role, "Admin")
            }, "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        [Test]
        public async Task UploadFile_ReturnsCreated_WhenSuccess()
        {
            var formFileMock = new Mock<IFormFile>();
            var uploadDto = new FileUploadDto { File = formFileMock.Object };
            var resultString = "Uploaded";
            _fileArchiveServiceMock.Setup(s => s.UploadFile(uploadDto, It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(resultString);

            var result = await _controller.UploadFile(uploadDto);

            var created = result.Result as CreatedResult;
            Assert.That(created, Is.Not.Null);
            var response = created.Value as ApiResponseDto<object>;
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Message, Does.Contain("Uploaded"));
        }

        [Test]
        public async Task GetById_ReturnsBadRequest_WhenException()
        {
            var id = Guid.NewGuid();
            _fileArchiveServiceMock.Setup(s => s.GetById(id)).ThrowsAsync(new Exception("Not found"));

            var result = await _controller.GetById(id);

            var badRequest = result.Result as BadRequestObjectResult;
            Assert.That(badRequest, Is.Not.Null);
            var response = badRequest.Value as ApiResponseDto<object>;
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Message, Does.Contain("Cannot fetch FileArchive"));
        }

        [Test]
        public async Task DeleteFileArchive_ReturnsBadRequest_WhenException()
        {
            var id = Guid.NewGuid();
            _fileArchiveServiceMock.Setup(s => s.DeleteFileArchive(id, It.IsAny<string>()))
                .ThrowsAsync(new Exception("Delete failed"));

            var result = await _controller.DeleteFileArchive(id);

            var badRequest = result.Result as BadRequestObjectResult;
            Assert.That(badRequest, Is.Not.Null);
            var response = badRequest.Value as ApiResponseDto<object>;
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Message, Does.Contain("Cannot delete FileArchive"));
        }
    }
}