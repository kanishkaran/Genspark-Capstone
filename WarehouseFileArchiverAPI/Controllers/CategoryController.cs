using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WarehouseFileArchiverAPI.Interfaces;
using WarehouseFileArchiverAPI.Models.DTOs;
using WarehouseFileArchiverAPI.Models;

namespace WarehouseFileArchiverAPI.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [Authorize]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponseDto<object>>> Search([FromQuery] SearchQueryDto searchDto)
        {
            try
            {
                var result = await _categoryService.SearchCategories(searchDto);
                return ApiResponseDto<object>.SuccessReponse("Categories fetched successfully", result);
            }
            catch (Exception e)
            {
                return ApiResponseDto<object>.ErrorResponse("No categories found", new
                {
                    fields = e.Message
                });
            }
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponseDto<object>>> GetById(Guid id)
        {
            try
            {
                var category = await _categoryService.GetById(id);
                return ApiResponseDto<object>.SuccessReponse("Category fetched successfully", category);
            }
            catch (Exception e)
            {
                return ApiResponseDto<object>.ErrorResponse("Category not found", new
                {
                    fields = e.Message
                });
            }
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponseDto<object>>> AddCategory(CategoryAddRequestDto category)
        {
            try
            {
                var newCategory = await _categoryService.AddCategory(category, User.FindFirstValue(ClaimTypes.NameIdentifier));

                return ApiResponseDto<object>.SuccessReponse("Category Added Successfully", newCategory);
            }
            catch (Exception e)
            {
                return ApiResponseDto<object>.ErrorResponse("Category Cannot be added", new
                {
                    fields = e.Message
                });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponseDto<object>>> UpdateCategory(Guid id, CategoryAddRequestDto category)
        {
            try
            {
                var updatedCategory = await _categoryService.UpdateCategory(id, category, User.FindFirstValue(ClaimTypes.NameIdentifier));

                return ApiResponseDto<object>.SuccessReponse("Category Updated Successfully", updatedCategory);
            }
            catch (Exception e)
            {
                return ApiResponseDto<object>.ErrorResponse("Category Cannot be updated", new
                {
                    fields = e.Message
                });
            }
        }
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponseDto<object>>> DeleteCategory(Guid id)
        {
            try
            {
                var result = await _categoryService.DeleteCategory(id, User.FindFirstValue(ClaimTypes.NameIdentifier));
                return ApiResponseDto<object>.SuccessReponse("Category deleted successfully", result);
            }
            catch (Exception e)
            {
                return ApiResponseDto<object>.ErrorResponse("Category cannot be deleted", new
                {
                    fields = e.Message
                });
            }
        }



    }
}