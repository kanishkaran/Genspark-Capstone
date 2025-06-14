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
    public class RoleCategoryAccessControllerTest
    {
        private Mock<IRoleCategoryAccessService> _serviceMock;
        private RoleCategoryAccessController _controller;

        [SetUp]
        public void SetUp()
        {
            _serviceMock = new Mock<IRoleCategoryAccessService>();
            _controller = new RoleCategoryAccessController(_serviceMock.Object);

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
            var page = new PaginationDto<RoleCategoryAccessListDto>();
            _serviceMock.Setup(s => s.SearchRoleCategories(dto)).ReturnsAsync(page);

            var result = await _controller.Search(dto);

            var ok = result.Result as OkObjectResult;
            Assert.That(ok, Is.Not.Null);
            var response = ok.Value as ApiResponseDto<PaginationDto<RoleCategoryAccessListDto>>;
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Message, Does.Contain("Fetched role-category accesses successfully"));
        }

        [Test]
        public async Task GetById_ReturnsNotFound_WhenException()
        {
            var id = Guid.NewGuid();
            _serviceMock.Setup(s => s.GetByIdAsync(id)).ThrowsAsync(new Exception("Not found"));

            var result = await _controller.GetById(id);

            var notFound = result.Result as NotFoundObjectResult;
            Assert.That(notFound, Is.Not.Null);
            var response = notFound.Value as ApiResponseDto<object>;
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Message, Does.Contain("Role-category access not found"));
        }

        [Test]
        public async Task Add_ReturnsOk_WhenSuccess()
        {
            var addDto = new RoleCategoryAccessAddRequestDto();
            var access = new RoleCategoryAccess();
            _serviceMock.Setup(s => s.AddAsync(addDto, It.IsAny<string>())).ReturnsAsync(access);

            var result = await _controller.Add(addDto);

            var ok = result.Result as OkObjectResult;
            Assert.That(ok, Is.Not.Null);
            var response = ok.Value as ApiResponseDto<RoleCategoryAccess>;
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Message, Does.Contain("Role-category access added successfully"));
        }
    }
}