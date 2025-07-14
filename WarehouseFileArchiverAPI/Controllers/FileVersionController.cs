using System;
using System.Collections.Generic;
using System.Linq;
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
    public class FileVersionController : ControllerBase
    {

        private readonly IFileVersionService _fileVersionService;

        public FileVersionController(IFileVersionService fileVersion)
        {
            _fileVersionService = fileVersion;
        }
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<ApiResponseDto<object>>> SearchFileVersions()
        {
            try
            {
                var result = await _fileVersionService.GetAllVersions();
                return Ok(ApiResponseDto<object>.SuccessReponse("FileVersions fetched successfully", result));
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
        public async Task<ActionResult<ApiResponseDto<object>>> GetFileVersionByArchiveId(Guid id)
        {

            try
            {
                var result = await _fileVersionService.GetFileVersionsByArchiveId(id);
                return Ok(ApiResponseDto<object>.SuccessReponse("FileVersions fetched successfully", result));
            }
            catch (Exception e)
            {
                return BadRequest(ApiResponseDto<object>.ErrorResponse("Cannot fetch FileArchives", new
                {
                    fields = e.Message
                }));
            }
        }
    }
}