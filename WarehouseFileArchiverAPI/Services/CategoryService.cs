using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Office2010.Excel;
using WarehouseFileArchiverAPI.Exceptions;
using WarehouseFileArchiverAPI.Interfaces;
using WarehouseFileArchiverAPI.Mappers;
using WarehouseFileArchiverAPI.Models;
using WarehouseFileArchiverAPI.Models.DTOs;

namespace WarehouseFileArchiverAPI.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IRepository<Guid, Category> _categoryRepository;
        private readonly IRepository<Guid, AccessLevel> _accessLevelRepository;
        private readonly CategoryMapper _mapper;
        private readonly IAuditLogService _auditLogService;

        public CategoryService(
            IRepository<Guid, Category> categoryRepository,
            IRepository<Guid, AccessLevel> accessLevelRepository,
            IAuditLogService auditLogService)
        {
            _categoryRepository = categoryRepository;
            _accessLevelRepository = accessLevelRepository;
            _mapper = new();
            _auditLogService = auditLogService;
        }

        public async Task<Category> AddCategory(CategoryAddRequestDto category, string currUser)
        {
            try
            {
                var allAccess = await _accessLevelRepository.GetAllAsync();
                var access = allAccess.FirstOrDefault(ac => ac.Access.ToLower() == category.AccessLevel.ToLower())
                    ?? throw new InvalidAccessLevelException($"The access Level {category.AccessLevel} is invalid");

                var categories = await _categoryRepository.GetAllAsync();
                var newCategory = categories.FirstOrDefault(c => c.CategoryName.ToLower() == category.CategoryName.ToLower());
                if (newCategory != null)
                {
                    if (!newCategory.IsDeleted)
                        throw new Exception($"Category: {category.CategoryName} already Exists");

                    newCategory.IsDeleted = false;
                    newCategory.AccessLevelId = access.Id;
                    
                    var updated = await _categoryRepository.UpdateAsync(newCategory.Id, newCategory);

                    await _auditLogService.LogAsync(
                        "Category",
                        updated.Id,
                        "Reactivate",
                        currUser ?? "system",
                        JsonSerializer.Serialize(category)
                    );

                    return updated;
                }

                newCategory = _mapper.MapCategoryAddDtoToCategory(category);
                newCategory.AccessLevelId = access.Id;

                var added = await _categoryRepository.AddAsync(newCategory);

                await _auditLogService.LogAsync(
                    "Category",
                    added.Id,
                    "Add",
                    currUser ?? "system",
                    JsonSerializer.Serialize(category)
                );

                return added;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<CategoryListDto> UpdateCategory(Guid id, CategoryAddRequestDto categoryDto, string currUser)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null || category.IsDeleted)
                throw new CategoryNotFoundException($"Category with id: {id} was not found or deleted");

            var allAccess = await _accessLevelRepository.GetAllAsync();
            var access = allAccess.FirstOrDefault(ac => ac.Access == categoryDto.AccessLevel && ac.IsActive)
                ?? throw new InvalidAccessLevelException($"The access Level {categoryDto.AccessLevel} is invalid or deleted");

            // var oldCategory = JsonSerializer.Serialize(category);

            category.CategoryName = categoryDto.CategoryName;
            category.AccessLevelId = access.Id;

            var updated = await _categoryRepository.UpdateAsync(id, category);
            var result = new CategoryListDto
            {
                CategoryName = updated.CategoryName,
                Access = access.Access
            };

            await _auditLogService.LogAsync(
                "Category",
                updated.Id,
                "Update",
                currUser ?? "system",
                $"Old: {JsonSerializer.Serialize(result)}, New: {JsonSerializer.Serialize(categoryDto)}"
            );

            return result;
        }

        public async Task<Category> GetById(Guid id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null || category.IsDeleted)
                throw new CategoryNotFoundException($"Category with id: {id} was not found or deleted");
            return category;
        }

        public async Task<PaginationDto<CategoryListDto>> SearchCategories(SearchQueryDto searchDto)
        {
            var categories = await _categoryRepository.GetAllAsync() ?? throw new CollectionEmptyException("No categories in the Database");

            if (!searchDto.IncludeInactive)
                categories = categories.Where(c => !c.IsDeleted);

            if (!string.IsNullOrWhiteSpace(searchDto.Search))
            {
                var search = searchDto.Search.ToLower();
                categories = categories.Where(c => c.CategoryName.ToLower().Contains(search) || c.AccessLevel.Access.ToLower().Contains(search));
            }

            categories = searchDto.SortBy?.ToLower() switch
            {
                "categoryname" => searchDto.Desc ? categories.OrderByDescending(c => c.CategoryName) : categories.OrderBy(c => c.CategoryName),
                _ => searchDto.Desc ? categories.OrderByDescending(c => c.Id) : categories.OrderBy(c => c.Id)
            };

            var totalRecords = categories.Count();
            if (totalRecords == 0)
                throw new CollectionEmptyException("No Category Matched");

            var items = categories
                .Skip((searchDto.Page - 1) * searchDto.PageSize)
                .Take(searchDto.PageSize)
                .Select(MapCategoryListDto)
                .ToList();

            return new PaginationDto<CategoryListDto>
            {
                Data = items,
                TotalRecords = totalRecords,
                Page = searchDto.Page,
                PageSize = searchDto.PageSize,
                TotalPages = (int)Math.Ceiling(totalRecords / (double)searchDto.PageSize)
            };
        }

        private CategoryListDto MapCategoryListDto(Category category)
        {
            return new CategoryListDto
            {
                Id = category.Id,
                CategoryName = category.CategoryName,
                Access = category.AccessLevel.Access,
                IsDeleted = category.IsDeleted
            };
        }

        public async Task<Category> GetCategoryByName(string categoryName)
        {
            try
            {
                var allCategory = await _categoryRepository.GetAllAsync();

                return allCategory.FirstOrDefault(c => c.CategoryName.ToLower() == categoryName.ToLower() && !c.IsDeleted)
                    ?? throw new CategoryNotFoundException($"No such category as {categoryName}");
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> DeleteCategory(Guid id, string currUser)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null || category.IsDeleted)
                throw new CategoryNotFoundException($"Category with id: {id} was not found or already deleted");

            category.IsDeleted = true;
            await _categoryRepository.UpdateAsync(id, category);

            await _auditLogService.LogAsync(
                "Category",
                category.Id,
                "Delete",
                currUser ?? "system",
                $"Category {category.CategoryName} marked as deleted."
            );

            return true;
        }

    }
}