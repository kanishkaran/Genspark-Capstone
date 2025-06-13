using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WarehouseFileArchiverAPI.Models.DTOs
{
    public class FileDownloadRequestDto
    {
        public string FileName { get; set; } = string.Empty;
        public int VersionNumber { get; set; }
    }
}