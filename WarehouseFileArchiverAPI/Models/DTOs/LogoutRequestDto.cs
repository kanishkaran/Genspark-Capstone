using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WarehouseFileArchiverAPI.Models.DTOs
{
    public class LogoutRequestDto
    {
        public string Username { get; set; } = string.Empty;
        public string AccessToken { get; set; } = string.Empty;
    }
}