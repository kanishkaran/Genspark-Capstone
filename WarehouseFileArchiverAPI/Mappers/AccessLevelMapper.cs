using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WarehouseFileArchiverAPI.Interfaces;
using WarehouseFileArchiverAPI.Models;
using WarehouseFileArchiverAPI.Models.DTOs;

namespace WarehouseFileArchiverAPI.Mappers
{
    public class AccessLevelMapper 
    {
        public AccessLevel MapAccessLevelAddDtoToAccessLevel(AccessLevelAddRequestDto access)
        {
            var currAccess = new AccessLevel()
            {
                Access = access.Access
            };

            return currAccess;
        }
    }
}