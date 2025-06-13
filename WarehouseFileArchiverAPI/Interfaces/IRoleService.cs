using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WarehouseFileArchiverAPI.Models;
using WarehouseFileArchiverAPI.Models.DTOs;

namespace WarehouseFileArchiverAPI.Interfaces
{
    public interface IRoleService
    {
        Task<PaginationDto<Role>> SearchRoles(SearchQueryDto searchDto);
        Task<Role> GetById(Guid id);
        Task<Role> AddRole(RoleAddRequestDto roleDto, string changedBy = "system");
        Task<Role> UpdateRole(Guid id, RoleAddRequestDto roleDto, string changedBy = "system");
        Task<bool> DeleteRole(Guid id, string changedBy = "system");

    }
}