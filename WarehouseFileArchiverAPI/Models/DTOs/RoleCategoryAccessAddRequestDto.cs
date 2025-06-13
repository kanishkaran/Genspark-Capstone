using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WarehouseFileArchiverAPI.Models.DTOs
{
    public class RoleCategoryAccessAddRequestDto
    {
        public string Role { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;

        public bool CanUpload { get; set; } = false;
        public bool CanDownload { get; set; } = false;
    }
}