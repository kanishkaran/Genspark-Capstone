using System;
using System.Security.Claims;
using System.Threading.Tasks;
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
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponseDto<object>>> Search([FromQuery] SearchQueryDto searchDto)
        {
            try
            {
                var result = await _roleService.SearchRoles(searchDto);
                return Ok(ApiResponseDto<object>.SuccessReponse("Roles fetched successfully", result));
            }
            catch (Exception e)
            {
                return BadRequest(ApiResponseDto<object>.ErrorResponse("No roles found", new
                {
                    fields = e.Message
                }));
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponseDto<object>>> GetById(Guid id)
        {
            try
            {
                var role = await _roleService.GetById(id);
                return Ok(ApiResponseDto<Role>.SuccessReponse("Fetched Role Successfully", role));
            }
            catch (Exception e)
            {
                return NotFound(ApiResponseDto<object>.ErrorResponse("Role not found", new
                {
                    fields = e.Message
                }));
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponseDto<object>>> AddRole(RoleAddRequestDto role)
        {
            try
            {
                var newRole = await _roleService.AddRole(role);
                return Ok(ApiResponseDto<Role>.SuccessReponse("Role Added Successfully", newRole));
            }
            catch (Exception e)
            {
                return BadRequest(ApiResponseDto<object>.ErrorResponse("Cannot Add Role", new
                {
                    fields = e.Message
                }));
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponseDto<object>>> UpdateRole(Guid id, RoleAddRequestDto role)
        {
            try
            {
                var updatedRole = await _roleService.UpdateRole(id, role);
                return Ok(ApiResponseDto<Role>.SuccessReponse("Role Updated Successfully", updatedRole));
            }
            catch (Exception e)
            {
                return BadRequest(ApiResponseDto<object>.ErrorResponse("Cannot Update Role", new
                {
                    fields = e.Message
                }));
            }
        }
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponseDto<object>>> DeleteRole(Guid id)
        {
            try
            {
                var result = await _roleService.DeleteRole(id, User.FindFirstValue(ClaimTypes.NameIdentifier));
                return Ok(ApiResponseDto<object>.SuccessReponse("Role deleted successfully", result));
            }
            catch (Exception e)
            {
                return BadRequest(ApiResponseDto<object>.ErrorResponse("Cannot delete Role", new
                {
                    fields = e.Message
                }));
            }
        }




    }
}