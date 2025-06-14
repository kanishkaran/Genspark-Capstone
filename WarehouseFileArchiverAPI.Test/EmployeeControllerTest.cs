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
    public class EmployeeControllerTest
    {
        private Mock<IEmployeeService> _employeeServiceMock;
        private EmployeeController _controller;

        [SetUp]
        public void SetUp()
        {
            _employeeServiceMock = new Mock<IEmployeeService>();
            _controller = new EmployeeController(_employeeServiceMock.Object);

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
        public async Task RegisterEmployee_ReturnsOk_WhenSuccess()
        {
            var addDto = new EmployeeAddRequestDto { Email = "new@user.com", FirstName = "New", LastName = "User" };
            var employee = new Employee { Id = Guid.NewGuid(), Email = addDto.Email, FirstName = addDto.FirstName, LastName = addDto.LastName, IsActive = true };
            _employeeServiceMock.Setup(s => s.RegisterEmployee(addDto)).ReturnsAsync(employee);

            var result = await _controller.RegisterEmployee(addDto);

            var ok = result.Result as OkObjectResult;
            Assert.That(ok, Is.Not.Null);
            var response = ok.Value as ApiResponseDto<object>;
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Message, Does.Contain("Employee Added Successfully"));
        }

        [Test]
        public async Task GetById_ReturnsNotFound_WhenException()
        {
            var id = Guid.NewGuid();
            _employeeServiceMock.Setup(s => s.GetById(id)).ThrowsAsync(new Exception("Not found"));

            var result = await _controller.GetById(id);

            var notFound = result.Result as NotFoundObjectResult;
            Assert.That(notFound, Is.Not.Null);
            var response = notFound.Value as ApiResponseDto<object>;
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Message, Does.Contain("Employee not found"));
        }

        [Test]
        public async Task UpdateEmployee_ReturnsBadRequest_WhenException()
        {
            var id = Guid.NewGuid();
            var updateDto = new EmployeeUpdateRequestDto { FirstName = "Fail", LastName = "Case", Email = "fail@case.com" };
            _employeeServiceMock.Setup(s => s.UpdateEmployee(id, updateDto, It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new Exception("Update failed"));

            var result = await _controller.UpdateEmployee(id, updateDto);

            var badRequest = result.Result as BadRequestObjectResult;
            Assert.That(badRequest, Is.Not.Null);
            var response = badRequest.Value as ApiResponseDto<object>;
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Message, Does.Contain("Employee cannot be updated"));
        }
       
    }
}