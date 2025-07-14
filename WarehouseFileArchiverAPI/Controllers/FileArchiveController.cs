using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WarehouseFileArchiverAPI.Interfaces;
using WarehouseFileArchiverAPI.Models.DTOs;

namespace WarehouseFileArchiverAPI.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class FileArchiveController : ControllerBase
    {
        private readonly IFileArchiveService _fileArchiveService;

        public FileArchiveController(IFileArchiveService fileArchiveService)
        {
            _fileArchiveService = fileArchiveService;
        }
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<ApiResponseDto<object>>> SearchFileArchives([FromQuery] SearchQueryDto searchDto)
        {
            try
            {
                var result = await _fileArchiveService.SearchFileArchives(searchDto);
                return Ok(ApiResponseDto<object>.SuccessReponse("FileArchives fetched successfully", result));
            }
            catch (Exception e)
            {
                return BadRequest(ApiResponseDto<object>.ErrorResponse("Cannot fetch FileArchives", new
                {
                    fields = e.Message
                }));
            }
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<ApiResponseDto<object>>> GetById(Guid id)
        {
            try
            {
                var result = await _fileArchiveService.GetById(id);
                return Ok(ApiResponseDto<object>.SuccessReponse("FileArchive fetched successfully", result));
            }
            catch (Exception e)
            {
                return BadRequest(ApiResponseDto<object>.ErrorResponse("Cannot fetch FileArchive", new
                {
                    fields = e.Message
                }));
            }
        }

        [HttpGet("Download")]
        [Authorize]
        public async Task<IActionResult> DownloadFile([FromQuery] FileDownloadRequestDto requestDto)
        {
            try
            {
                var role = User.FindFirst(ClaimTypes.Role)?.Value;
                var user = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var result = await _fileArchiveService.DownloadFile(requestDto.FileName, requestDto.VersionNumber, role, user);

                return File(result.FileContent, result.ContentType, result.FileName);
            }
            catch (Exception e)
            {

                return BadRequest(ApiResponseDto<object>.ErrorResponse("Cannot Upload file", new
                {
                    fields = e.Message
                }));
            }
        }

        //3rd
        [HttpPost("Upload")]
        [Authorize(Policy = "UploadAccess")]
        public async Task<ActionResult<ApiResponseDto<object>>> UploadFile(FileUploadDto uploadDto)
        {
            try
            {
                var user = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var role = User.FindFirst(ClaimTypes.Role)?.Value;
                var result = await _fileArchiveService.UploadFile(uploadDto, user, role);

                return Created("", ApiResponseDto<object>.SuccessReponse("Uploaded", result));
            }
            catch (Exception e)
            {

                return BadRequest(ApiResponseDto<object>.ErrorResponse("Cannot Upload file", new
                {
                    fields = e.Message
                }));
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponseDto<object>>> DeleteFileArchive(Guid id)
        {
            try
            {
                var user = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var result = await _fileArchiveService.DeleteFileArchive(id, user);
                return Ok(ApiResponseDto<object>.SuccessReponse("FileArchive deleted successfully", result));
            }
            catch (Exception e)
            {
                return BadRequest(ApiResponseDto<object>.ErrorResponse("Cannot delete FileArchive", new
                {
                    fields = e.Message
                }));
            }
        }
    }
}