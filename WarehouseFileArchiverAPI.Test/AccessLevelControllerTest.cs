using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using WarehouseFileArchiverAPI.Controllers;
using WarehouseFileArchiverAPI.Interfaces;
using WarehouseFileArchiverAPI.Models;
using WarehouseFileArchiverAPI.Models.DTOs;

namespace WarehouseFileArchiverAPI.Test
{
    [TestFixture]
    public class AccessLevelControllerTest
    {
        private Mock<IAccessLevelService> _serviceMock;
        private AccessLevelController _controller;

        [SetUp]
        public void SetUp()
        {
            _serviceMock = new Mock<IAccessLevelService>();
            _controller = new AccessLevelController(_serviceMock.Object);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "admin@email.com"),
                new Claim(ClaimTypes.Role, "Admin")
            }, "mock"));
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        [Test]
        public async Task Search_ReturnsOk_WhenSuccess()
        {
            var dto = new SearchQueryDto();
            var page = new PaginationDto<AccessLevel>();
            _serviceMock.Setup(s => s.SearchAccessLevel(dto)).ReturnsAsync(page);

            var result = await _controller.Search(dto);

            var ok = result.Result as OkObjectResult;
            Assert.That(ok, Is.Not.Null);
            var response = ok.Value as ApiResponseDto<PaginationDto<AccessLevel>>;
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Message, Does.Contain("Fetched access levels successfully"));
        }

        [Test]
        public async Task GetById_ReturnsNotFound_WhenException()
        {
            var id = Guid.NewGuid();
            _serviceMock.Setup(s => s.GetAccessLevelById(id)).ThrowsAsync(new Exception("Not found"));

            var result = await _controller.GetById(id);

            var notFound = result.Result as NotFoundObjectResult;
            Assert.That(notFound, Is.Not.Null);
            var response = notFound.Value as ApiResponseDto<object>;
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Message, Does.Contain("Access level not found"));
        }

        [Test]
        public async Task AddAccessLevel_ReturnsOk_WhenSuccess()
        {
            var addDto = new AccessLevelAddRequestDto();
            var accessLevel = new AccessLevel();
            _serviceMock.Setup(s => s.AddAccessLevel(addDto, It.IsAny<string>())).ReturnsAsync(accessLevel);

            var result = await _controller.AddAccessLevel(addDto);

            var ok = result.Result as OkObjectResult;
            Assert.That(ok, Is.Not.Null);
            var response = ok.Value as ApiResponseDto<object>;
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Message, Does.Contain("Access Level Successfully Added"));
        }
    }
}