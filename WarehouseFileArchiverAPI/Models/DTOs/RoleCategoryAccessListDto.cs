using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WarehouseFileArchiverAPI.Models.DTOs
{
    public class RoleCategoryAccessListDto
    {

        public string RoleName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public bool CanUpload { get; set; }
        public bool CanDownload { get; set; }
    }
}