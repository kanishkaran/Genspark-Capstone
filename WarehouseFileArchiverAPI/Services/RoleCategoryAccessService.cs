using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WarehouseFileArchiverAPI.Interfaces;
using WarehouseFileArchiverAPI.Models;
using WarehouseFileArchiverAPI.Models.DTOs;
using WarehouseFileArchiverAPI.Mappers;
using WarehouseFileArchiverAPI.Exceptions;

namespace WarehouseFileArchiverAPI.Services
{
    public class RoleCategoryAccessService : IRoleCategoryAccessService
    {
        private readonly IRepository<Guid, RoleCategoryAccess> _roleCategoryAccessRepository;
        private readonly IRepository<Guid, Role> _roleRepository;
        private readonly IRepository<Guid, Category> _categoryRepository;
        private readonly RoleCategoryAccessMapper _mapper;
        private readonly IAuditLogService _auditLogService;

        public RoleCategoryAccessService(
            IRepository<Guid, RoleCategoryAccess> repository,
            IRepository<Guid, Role> roleRepository,
            IRepository<Guid, Category> categoryRepository,
            IAuditLogService auditLogService)
        {
            _roleCategoryAccessRepository = repository;
            _roleRepository = roleRepository;
            _categoryRepository = categoryRepository;
            _mapper = new();
            _auditLogService = auditLogService;
        }

        public async Task<PaginationDto<RoleCategoryAccessListDto>> SearchRoleCategories(SearchQueryDto searchDto)
        {
            var allRoleCategories = await _roleCategoryAccessRepository.GetAllAsync();

            // Exclude deleted by default
            if (!searchDto.IncludeInactive)
                allRoleCategories = allRoleCategories.Where(r => !r.IsDeleted);

            allRoleCategories = ApplyFilters(allRoleCategories, searchDto);
            allRoleCategories = ApplySorting(allRoleCategories, searchDto);

            var totalRecords = allRoleCategories.Count();
            if (totalRecords == 0)
            {
                throw new CollectionEmptyException("No Role Category Matched");
            }
            var items = ApplyPagination(allRoleCategories, searchDto).ToList();

            return new PaginationDto<RoleCategoryAccessListDto>
            {
                Data = items,
                TotalRecords = totalRecords,
                Page = searchDto.Page,
                PageSize = searchDto.PageSize,
                TotalPages = (int)Math.Ceiling(totalRecords / (double)searchDto.PageSize)
            };
        }

        private IEnumerable<RoleCategoryAccess> ApplyFilters(IEnumerable<RoleCategoryAccess> query, SearchQueryDto searchDto)
        {
            if (!string.IsNullOrWhiteSpace(searchDto.Search))
            {
                var search = searchDto.Search.ToLower();
                query = query.Where(r =>
                    (r.Role != null && r.Role.RoleName.ToLower().Contains(search)) ||
                    (r.Category != null && r.Category.CategoryName.ToLower().Contains(search))
                );
            }
            return query;
        }

        private IEnumerable<RoleCategoryAccess> ApplySorting(IEnumerable<RoleCategoryAccess> query, SearchQueryDto searchDto)
        {
            return searchDto.SortBy?.ToLower() switch
            {
                "role" => searchDto.Desc
                    ? query.OrderByDescending(r => r.Role?.RoleName)
                    : query.OrderBy(r => r.Role?.RoleName),
                "category" => searchDto.Desc
                    ? query.OrderByDescending(r => r.Category?.CategoryName)
                    : query.OrderBy(r => r.Category?.CategoryName),
                _ => query
            };
        }

        private IEnumerable<RoleCategoryAccessListDto> ApplyPagination(IEnumerable<RoleCategoryAccess> query, SearchQueryDto searchDto)
        {
            return query
                .Skip((searchDto.Page - 1) * searchDto.PageSize)
                .Take(searchDto.PageSize)
                .Select(rc => new RoleCategoryAccessListDto
                {
                    RoleName = rc.Role?.RoleName ?? string.Empty,
                    CategoryName = rc.Category?.CategoryName ?? string.Empty,
                    CanUpload = rc.CanUpload,
                    CanDownload = rc.CanDownload
                });
        }

        public async Task<RoleCategoryAccess> GetByIdAsync(Guid id)
        {
            var entity = await _roleCategoryAccessRepository.GetByIdAsync(id);
            if (entity == null || entity.IsDeleted)
                throw new Exception($"RoleCategoryAccess with id: {id} was not found");
            return entity;
        }

        public async Task<RoleCategoryAccess> AddAsync(RoleCategoryAccessAddRequestDto dto, string currUser)
        {
            var roles = await _roleRepository.GetAllAsync();
            var role = roles.FirstOrDefault(r => r.RoleName.Equals(dto.Role, StringComparison.OrdinalIgnoreCase) && !r.IsDeleted)
                ?? throw new Exception($"Role '{dto.Role}' does not exist.");

            var categories = await _categoryRepository.GetAllAsync();
            var category = categories.FirstOrDefault(c => c.CategoryName.Equals(dto.Category, StringComparison.OrdinalIgnoreCase) && !c.IsDeleted)
                ?? throw new Exception($"Category '{dto.Category}' does not exist.");

            var roleCategories = await _roleCategoryAccessRepository.GetAllAsync();
            var roleCategory = roleCategories.FirstOrDefault(rc => rc.CategoryId == category.Id && rc.RoleId == role.Id);

            if (roleCategory != null)
            {
                if (!roleCategory.IsDeleted)
                    throw new Exception("Role Category Already Exist");

                roleCategory.IsDeleted = false;
                await _roleCategoryAccessRepository.UpdateAsync(roleCategory.Id, roleCategory);

                await _auditLogService.LogAsync(
                    "RoleCategoryAccess",
                    roleCategory.Id,
                    "Re-Activate",
                    currUser,
                    "Changed status to not deleted"
                );

                return roleCategory;
            }

            roleCategory = _mapper.MapAddDtoToRoleCategoryAccess(dto);
            roleCategory.RoleId = role.Id;
            roleCategory.CategoryId = category.Id;
            roleCategory.IsDeleted = false;

            await _auditLogService.LogAsync(
                "RoleCategory",
                roleCategory.Id,
                "Add",
                currUser,
                $"New Data Added {roleCategory}"
            );

            return await _roleCategoryAccessRepository.AddAsync(roleCategory);
        }

        public async Task<RoleCategoryAccess> UpdateAsync(Guid id, RoleCategoryAccessAddRequestDto dto, string currUser)
        {
            var roleCategory = await _roleCategoryAccessRepository.GetByIdAsync(id);
            if (roleCategory == null || roleCategory.IsDeleted)
                throw new Exception($"RoleCategoryAccess with id: {id} was not found");
            var oldCategory = roleCategory;

            var roles = await _roleRepository.GetAllAsync();
            var role = roles.FirstOrDefault(r => r.RoleName.Equals(dto.Role, StringComparison.OrdinalIgnoreCase) && !r.IsDeleted);
            if (role == null)
                throw new RoleNotFoundException($"Role '{dto.Role}' does not exist.");

            var categories = await _categoryRepository.GetAllAsync();
            var category = categories.FirstOrDefault(c => c.CategoryName.Equals(dto.Category, StringComparison.OrdinalIgnoreCase) && !c.IsDeleted);
            if (category == null)
                throw new CategoryNotFoundException($"Category '{dto.Category}' does not exist.");

            roleCategory.RoleId = role.Id;
            roleCategory.CategoryId = category.Id;
            roleCategory.CanUpload = dto.CanUpload;
            roleCategory.CanDownload = dto.CanDownload;

            await _auditLogService.LogAsync(
                "RoleCategory",
                roleCategory.Id,
                "Update",
                currUser,
                $"Old Data: {oldCategory} New Data: {roleCategory}"
            );

            return await _roleCategoryAccessRepository.UpdateAsync(id, roleCategory);
        }

        public async Task<bool> DeleteAsync(Guid id, string currUser)
        {
            var roleCategory = await _roleCategoryAccessRepository.GetByIdAsync(id);
            if (roleCategory == null || roleCategory.IsDeleted)
                throw new Exception($"RoleCategoryAccess with id: {id} was not found");

            roleCategory.IsDeleted = true;
            await _roleCategoryAccessRepository.UpdateAsync(id, roleCategory);
            await _auditLogService.LogAsync(
                "RoleCategory",
                roleCategory.Id,
                "Delete",
                currUser,
                $"Rolecategory {roleCategory} marked as deleted."
            );
            return true;
        }
    }
}