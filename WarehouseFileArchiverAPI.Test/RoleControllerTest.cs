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
    public class RoleControllerTest
    {
        private Mock<IRoleService> _serviceMock;
        private RoleController _controller;

        [SetUp]
        public void SetUp()
        {
            _serviceMock = new Mock<IRoleService>();
            _controller = new RoleController(_serviceMock.Object);

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
            _serviceMock.Setup(s => s.SearchRoles(dto)).ReturnsAsync(new PaginationDto<Role>());

            var result = await _controller.Search(dto);

            var ok = result.Result as OkObjectResult;
            Assert.That(ok, Is.Not.Null);
            var response = ok.Value as ApiResponseDto<object>;
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Message, Does.Contain("Roles fetched successfully"));
        }

        [Test]
        public async Task GetById_ReturnsNotFound_WhenException()
        {
            var id = Guid.NewGuid();
            _serviceMock.Setup(s => s.GetById(id)).ThrowsAsync(new Exception("Not found"));

            var result = await _controller.GetById(id);

            var notFound = result.Result as NotFoundObjectResult;
            Assert.That(notFound, Is.Not.Null);
            var response = notFound.Value as ApiResponseDto<object>;
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Message, Does.Contain("Role not found"));
        }

    }
}