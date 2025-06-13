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
    public class UserControllerTest
    {
        private Mock<IUserService> _userServiceMock;
        private UserController _controller;

        [SetUp]
        public void Setup()
        {
            _userServiceMock = new Mock<IUserService>();
            _controller = new UserController(_userServiceMock.Object);
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "admin"),
                new Claim(ClaimTypes.Role, "Admin")
            }, "mock"));
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        [Test]
        public async Task GetById_ReturnsOk_WhenUserExists()
        {
            _userServiceMock.Setup(s => s.GetById("test")).ReturnsAsync(new UserListDto { Username = "test" });

            var result = await _controller.GetById("test");

            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public async Task AddUser_ReturnsOk_WhenSuccess()
        {
            _userServiceMock.Setup(s => s.AddUser(It.IsAny<UserAddRequestDto>(), "admin")).ReturnsAsync(new UserListDto { Username = "test" });

            var result = await _controller.AddUser(new UserAddRequestDto());

            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public async Task DeleteUser_ReturnsOk_WhenSuccess()
        {
            _userServiceMock.Setup(s => s.DeleteUser("test", "admin")).ReturnsAsync("Deleted");

            var result = await _controller.DeleteUser("test");

            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        }
    }
}