using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WarehouseFileArchiverAPI.Models.DTOs
{
    public class RoleAddRequestDto
    {
        public string Role { get; set; } = string.Empty;
        public string AccessLevel { get; set; } = string.Empty;
    }
}