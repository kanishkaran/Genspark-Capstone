using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WarehouseFileArchiverAPI.Models.DTOs
{
    public class MediaTypeAddRequestDto
    {
        public string TypeName { get; set; } = string.Empty;
        public string Extension { get; set; } = string.Empty;        
    }
}