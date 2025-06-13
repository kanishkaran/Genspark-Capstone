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
    public class MediaTypeController : ControllerBase
    {
        private readonly IMediaTypeService _mediaTypeService;

        public MediaTypeController(IMediaTypeService mediaTypeService)
        {
            _mediaTypeService = mediaTypeService;
        }

        [HttpGet]

        public async Task<ActionResult<ApiResponseDto<object>>> GetMediaTypes([FromQuery] SearchQueryDto query)
        {
            try
            {
                var response = await _mediaTypeService.SearchMediaTypes(query);
                return Ok(ApiResponseDto<object>.SuccessReponse("Searched Media Types Successfully", response));
            }
            catch (Exception e)
            {
                return BadRequest(ApiResponseDto<object>.ErrorResponse("Could not fetch media types", new
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
                var mediaType = await _mediaTypeService.GetById(id);
                return Ok(ApiResponseDto<object>.SuccessReponse("Fetched MediaType Successfully", mediaType));
            }
            catch (Exception e)
            {
                return NotFound(ApiResponseDto<object>.ErrorResponse("MediaType not found", new
                {
                    fields = e.Message
                }));
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponseDto<object>>> AddMediaType(MediaTypeAddRequestDto media)
        {
            try
            {
                var newType = await _mediaTypeService.AddMediaType(media, User.FindFirstValue(ClaimTypes.NameIdentifier));
                return Ok(ApiResponseDto<MediaType>.SuccessReponse("MediaType Added Successfully", newType));
            }
            catch (Exception e)
            {
                return BadRequest(ApiResponseDto<object>.ErrorResponse("MediaType Cannot be added", new
                {
                    fields = e.Message
                }));
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponseDto<object>>> UpdateMediaType(Guid id, MediaTypeAddRequestDto media)
        {
            try
            {
                var updatedType = await _mediaTypeService.UpdateMediaType(id, media, User.FindFirstValue(ClaimTypes.NameIdentifier));
                return Ok(ApiResponseDto<MediaType>.SuccessReponse("MediaType Updated Successfully", updatedType));
            }
            catch (Exception e)
            {
                return BadRequest(ApiResponseDto<object>.ErrorResponse("MediaType Cannot be updated", new
                {
                    fields = e.Message
                }));
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponseDto<object>>> DeleteMediaType(Guid id)
        {
            try
            {
                var result = await _mediaTypeService.DeleteMediaType(id, User.FindFirstValue(ClaimTypes.NameIdentifier));
                return Ok(ApiResponseDto<object>.SuccessReponse("MediaType deleted successfully", result));
            }
            catch (Exception e)
            {
                return BadRequest(ApiResponseDto<object>.ErrorResponse("MediaType cannot be deleted", new
                {
                    fields = e.Message
                }));
            }
        }
    }
}