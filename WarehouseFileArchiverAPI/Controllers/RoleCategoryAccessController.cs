using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WarehouseFileArchiverAPI.Interfaces;
using WarehouseFileArchiverAPI.Models.DTOs;
using WarehouseFileArchiverAPI.Models;
using System.Security.Claims;

namespace WarehouseFileArchiverAPI.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [Authorize]
    public class RoleCategoryAccessController : ControllerBase
    {
        private readonly IRoleCategoryAccessService _service;

        public RoleCategoryAccessController(IRoleCategoryAccessService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponseDto<object>>> Search([FromQuery] SearchQueryDto searchDto)
        {
            try
            {
                var result = await _service.SearchRoleCategories(searchDto);
                return Ok(ApiResponseDto<PaginationDto<RoleCategoryAccessListDto>>.SuccessReponse("Fetched role-category accesses successfully", result));
            }
            catch (Exception e)
            {
                return BadRequest(ApiResponseDto<object>.ErrorResponse("Cannot fetch role-category accesses", new { fields = e.Message }));
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponseDto<object>>> GetById(Guid id)
        {
            try
            {
                var result = await _service.GetByIdAsync(id);
                return Ok(ApiResponseDto<RoleCategoryAccess>.SuccessReponse("Fetched role-category access successfully", result));
            }
            catch (Exception e)
            {
                return NotFound(ApiResponseDto<object>.ErrorResponse("Role-category access not found", new { fields = e.Message }));
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponseDto<object>>> Add([FromBody] RoleCategoryAccessAddRequestDto roleCategory)
        {
            try
            {
                var result = await _service.AddAsync(roleCategory, User.FindFirstValue(ClaimTypes.NameIdentifier));
                return Ok(ApiResponseDto<RoleCategoryAccess>.SuccessReponse("Role-category access added successfully", result));
            }
            catch (Exception e)
            {
                return BadRequest(ApiResponseDto<object>.ErrorResponse("Cannot add role-category access", new { fields = e.Message }));
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponseDto<object>>> Update(Guid id, [FromBody] RoleCategoryAccessAddRequestDto roleCategory)
        {
            try
            {
                var result = await _service.UpdateAsync(id, roleCategory, User.FindFirstValue(ClaimTypes.NameIdentifier));
                return Ok(ApiResponseDto<RoleCategoryAccess>.SuccessReponse("Role-category access updated successfully", result));
            }
            catch (Exception e)
            {
                return BadRequest(ApiResponseDto<object>.ErrorResponse("Cannot update role-category access", new { fields = e.Message }));
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponseDto<object>>> Delete(Guid id)
        {
            try
            {
                var result = await _service.DeleteAsync(id, User.FindFirstValue(ClaimTypes.NameIdentifier));
                return Ok(ApiResponseDto<object>.SuccessReponse("Role-category access deleted successfully", result));
            }
            catch (Exception e)
            {
                return BadRequest(ApiResponseDto<object>.ErrorResponse("Cannot delete role-category access", new { fields = e.Message }));
            }
        }
    }
}