using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WarehouseFileArchiverAPI.Models.DTOs
{
    public class CategoryListDto
    {
        public string CategoryName { get; set; } = string.Empty;
        public string Access { get; set; } = string.Empty;
    }
}