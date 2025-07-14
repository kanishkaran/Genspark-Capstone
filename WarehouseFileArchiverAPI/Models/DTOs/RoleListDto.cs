using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WarehouseFileArchiverAPI.Models.DTOs
{
    public class RoleListDto
    {
        public Guid Id { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public string AccessLevel { get; set; } = string.Empty;
        public bool IsDeleted { get; set; }
    }
}