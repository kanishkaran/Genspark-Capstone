using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WarehouseFileArchiverAPI.Models.DTOs
{
    public class FileUploadDto
    {
        public IFormFile? File { get; set; }
        public string Category { get; set; } = string.Empty;
    }
}