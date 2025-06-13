using System;
using WarehouseFileArchiverAPI.Models;
using WarehouseFileArchiverAPI.Models.DTOs;

namespace WarehouseFileArchiverAPI.Mappers
{
    public class RoleCategoryAccessMapper
    {
        public RoleCategoryAccess MapAddDtoToRoleCategoryAccess(RoleCategoryAccessAddRequestDto dto)
        {
            return new RoleCategoryAccess
            {
                CanUpload = dto.CanUpload,
                CanDownload = dto.CanDownload
            };
        }
    }
}