using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WarehouseFileArchiverAPI.Models.DTOs
{
    public class CurrentUserDto
    {
        public string UserName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }
}