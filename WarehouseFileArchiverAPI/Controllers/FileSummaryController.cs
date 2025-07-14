
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WarehouseFileArchiverAPI.Interfaces;
namespace WarehouseFileArchiverAPI.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [Authorize]
    public class FileSummaryController : ControllerBase
    {

        private readonly IFileSummaryService _fileSummaryService;


        public FileSummaryController(IFileSummaryService fileSummaryService)
        {
            _fileSummaryService = fileSummaryService;


        }

        [HttpGet("summarize")]
        public async Task<IActionResult> SummarizeByFileName([FromQuery] string fileName)
        {
            try
            {
                var role = User.FindFirstValue(ClaimTypes.Role);
                var summary = await _fileSummaryService.SummarizeByFileNameAsync(fileName, role);
                return Ok(new { FileName = fileName, Summary = summary });
            }
            catch (Exception ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> SemanticSearchOnFileName([FromQuery]string query)
        {
            try
            {
                var role = User.FindFirstValue(ClaimTypes.Role);
                var fileName = await _fileSummaryService.SemanticSearchForFileName(query, role);
                return Ok(fileName);
            }
            catch (Exception ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }


    }
}