using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WarehouseFileArchiverAPI.Interfaces;
using WarehouseFileArchiverAPI.Models;
using WarehouseFileArchiverAPI.Models.DTOs;

namespace WarehouseFileArchiverAPI.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [Authorize]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponseDto<object>>> Search([FromQuery] EmployeeSearchDto searchDto)
        {
            try
            {
                var result = await _employeeService.SearchEmployee(searchDto);
                var response = ApiResponseDto<PaginationDto<Employee>>.SuccessReponse("Fetched employees successfully", result);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseDto<object>.ErrorResponse("Failed to fetch employees", new
                {
                    fields = ex.Message
                }));
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponseDto<object>>> GetById(Guid id)
        {
            try
            {
                var employee = await _employeeService.GetById(id);
                return Ok(ApiResponseDto<Employee>.SuccessReponse("Fetched employee successfully", employee));
            }
            catch (Exception ex)
            {
                return NotFound(ApiResponseDto<object>.ErrorResponse("Employee not found", new
                {
                    fields = ex.Message
                }));
            }
        }


        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponseDto<object>>> RegisterEmployee(EmployeeAddRequestDto employee)
        {
            try
            {
                var data = await _employeeService.RegisterEmployee(employee);
                return Ok(ApiResponseDto<object>.SuccessReponse("Employee Added Successfully", data));
            }
            catch (Exception e)
            {
                return BadRequest(ApiResponseDto<object>.ErrorResponse("Employee cannot be added", new
                {
                    fields = e.Message
                }));
            }
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<ApiResponseDto<object>>> UpdateEmployee(Guid id, EmployeeUpdateRequestDto employeeDto)
        {
            try
            {
                var user = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var role = User.FindFirstValue(ClaimTypes.Role);
                var updated = await _employeeService.UpdateEmployee(id, employeeDto, user, role);
                return Ok(ApiResponseDto<object>.SuccessReponse("Employee updated successfully", updated));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseDto<object>.ErrorResponse("Employee cannot be updated", new
                {
                    fields = ex.Message
                }));
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponseDto<object>>> DeleteEmployee(Guid id)
        {
            try
            {
                var message = await _employeeService.DeleteEmployee(id, User.FindFirstValue(ClaimTypes.NameIdentifier));
                return Ok(ApiResponseDto<object>.SuccessReponse("Deleted Action Successful", message));
            }
            catch (Exception ex)
            {
                return NotFound(ApiResponseDto<object>.ErrorResponse("Employee cannot be deleted", new
                {
                    fields = ex.Message
                }));
            }
        }
    }
}