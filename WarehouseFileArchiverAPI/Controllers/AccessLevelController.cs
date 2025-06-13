using System;
using System.Collections.Generic;
using System.Linq;
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
    public class AccessLevelController : ControllerBase
    {
        private readonly IAccessLevelService _accessLevelService;

        public AccessLevelController(IAccessLevelService accessLevelService)
        {
            _accessLevelService = accessLevelService;
        }

        [HttpGet]

        public async Task<ActionResult<ApiResponseDto<object>>> Search([FromQuery] SearchQueryDto searchDto)
        {
            try
            {
                var result = await _accessLevelService.SearchAccessLevel(searchDto);
                var response = ApiResponseDto<PaginationDto<AccessLevel>>.SuccessReponse("Fetched access levels successfully", result);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseDto<object>.ErrorResponse("Failed to fetch access levels", new
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
                var accessLevel = await _accessLevelService.GetAccessLevelById(id);
                return Ok(ApiResponseDto<AccessLevel>.SuccessReponse("Fetched access level successfully", accessLevel));
            }
            catch (Exception ex)
            {
                return NotFound(ApiResponseDto<object>.ErrorResponse("Access level not found", new
                {
                    fields = ex.Message
                }));
            }
        }


        [HttpPost("")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponseDto<object>>> AddAccessLevel(AccessLevelAddRequestDto accessLevel)
        {
            try
            {
                var access = await _accessLevelService.AddAccessLevel(accessLevel, User.FindFirstValue(ClaimTypes.NameIdentifier));
                return Ok(ApiResponseDto<object>.SuccessReponse("Access Level Successfully Added", access));
            }
            catch (Exception e)
            {
                return BadRequest(ApiResponseDto<object>.ErrorResponse("Cannot Add Access Level", new
                {
                    fields = e.Message
                }));
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponseDto<object>>> Delete(Guid id)
        {
            try
            {
                var message = await _accessLevelService.DeleteAccessLevel(id, User.FindFirstValue(ClaimTypes.NameIdentifier));
                return Ok(ApiResponseDto<object>.SuccessReponse("Deleted Action Successful", message));
            }
            catch (Exception ex)
            {
                return NotFound(ApiResponseDto<object>.ErrorResponse("Cannot delete access level", new
                {
                    fields = ex.Message
                }));
            }
        }
    }
}