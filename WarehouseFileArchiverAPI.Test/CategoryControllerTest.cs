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
    public class CategoryControllerTest
    {
        private Mock<ICategoryService> _serviceMock;
        private CategoryController _controller;

        [SetUp]
        public void SetUp()
        {
            _serviceMock = new Mock<ICategoryService>();
            _controller = new CategoryController(_serviceMock.Object);

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
            var dto = new SearchQueryDto { Search = "finance", Page = 1, PageSize = 10 };
            var dummyCategories = new PaginationDto<Category>
            {
                Data = new[] { new Category { Id = Guid.NewGuid(), CategoryName = "Finance", IsDeleted = false } },
                TotalRecords = 1,
                Page = 1,
                PageSize = 10
            };
            _serviceMock.Setup(s => s.SearchCategories(dto)).ReturnsAsync(dummyCategories);

            var result = await _controller.Search(dto);

            var ok = result.Result as OkObjectResult;
            Assert.That(ok, Is.Not.Null);
            var response = ok.Value as ApiResponseDto<object>;
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Message, Does.Contain("Categories fetched successfully"));
            Assert.That(response.Data.ToString(), Does.Contain("Finance"));
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
            Assert.That(response.Message, Does.Contain("Category not found"));
        }

        [Test]
        public async Task AddCategory_ReturnsOk_WhenSuccess()
        {
            var addDto = new CategoryAddRequestDto { CategoryName = "HR", AccessLevel = "Staff" };
            var category = new Category { Id = Guid.NewGuid(), CategoryName = "HR", IsDeleted = false };
            _serviceMock.Setup(s => s.AddCategory(addDto, It.IsAny<string>())).ReturnsAsync(category);

            var result = await _controller.AddCategory(addDto);

            var ok = result.Result as OkObjectResult;
            Assert.That(ok, Is.Not.Null);
            var response = ok.Value as ApiResponseDto<object>;
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Message, Does.Contain("Category Added Successfully"));
            Assert.That(response.Data.ToString(), Does.Contain("HR"));
        }
    }
}