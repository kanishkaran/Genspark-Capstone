
using WarehouseFileArchiverAPI.Models;
using WarehouseFileArchiverAPI.Models.DTOs;

namespace WarehouseFileArchiverAPI.Interfaces
{
    public interface ICategoryService
    {
        Task<PaginationDto<Category>> SearchCategories(SearchQueryDto searchDto);
        Task<Category> GetById(Guid id);
        Task<Category> AddCategory(CategoryAddRequestDto category, string currUser);
        Task<CategoryListDto> UpdateCategory(Guid id, CategoryAddRequestDto categoryDto, string currUser);
        Task<Category> GetCategoryByName(string categoryName);
        Task<bool> DeleteCategory(Guid id, string currUser);
    }
}