using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WarehouseFileArchiverAPI.Models;
using WarehouseFileArchiverAPI.Models.DTOs;

namespace WarehouseFileArchiverAPI.Interfaces
{
    public interface IAccessLevelService
    {
        Task<PaginationDto<AccessLevel>> SearchAccessLevel(SearchQueryDto searchDto);
        Task<AccessLevel> GetAccessLevelById(Guid id);
        Task<AccessLevel> AddAccessLevel(AccessLevelAddRequestDto access, string currUser);
        Task<string> DeleteAccessLevel(Guid id, string currUser);
    }
}