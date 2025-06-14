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
    public class MediaTypeControllerTest
    {
        private Mock<IMediaTypeService> _serviceMock;
        private MediaTypeController _controller;

        [SetUp]
        public void SetUp()
        {
            _serviceMock = new Mock<IMediaTypeService>();
            _controller = new MediaTypeController(_serviceMock.Object);

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
        public async Task GetMediaTypes_ReturnsOk_WhenSuccess()
        {
            var dto = new SearchQueryDto();
            _serviceMock.Setup(s => s.SearchMediaTypes(dto)).ReturnsAsync(new PaginationDto<MediaType>());

            var result = await _controller.GetMediaTypes(dto);

            var ok = result.Result as OkObjectResult;
            Assert.That(ok, Is.Not.Null);
            var response = ok.Value as ApiResponseDto<object>;
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Message, Does.Contain("Searched Media Types Successfully"));
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
            Assert.That(response.Message, Does.Contain("MediaType not found"));
        }

        [Test]
        public async Task AddMediaType_ReturnsOk_WhenSuccess()
        {
            var addDto = new MediaTypeAddRequestDto();
            var mediaType = new MediaType();
            _serviceMock.Setup(s => s.AddMediaType(addDto, It.IsAny<string>())).ReturnsAsync(mediaType);

            var result = await _controller.AddMediaType(addDto);

            var ok = result.Result as OkObjectResult;
            Assert.That(ok, Is.Not.Null);
            var response = ok.Value as ApiResponseDto<MediaType>;
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Message, Does.Contain("MediaType Added Successfully"));
        }
    }
}