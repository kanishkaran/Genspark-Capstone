using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WarehouseFileArchiverAPI.Models;
using WarehouseFileArchiverAPI.Models.DTOs;

namespace WarehouseFileArchiverAPI.Interfaces
{
    public interface IRoleCategoryAccessService
    {
        Task<PaginationDto<RoleCategoryAccessListDto>> SearchRoleCategories(SearchQueryDto searchDto);
        Task<RoleCategoryAccess> GetByIdAsync(Guid id);
        Task<RoleCategoryAccess> AddAsync(RoleCategoryAccessAddRequestDto dto, string currUser);
        Task<RoleCategoryAccess> UpdateAsync(Guid id, RoleCategoryAccessAddRequestDto dto, string currUser);
        Task<bool> DeleteAsync(Guid id, string currUser);
    }
}