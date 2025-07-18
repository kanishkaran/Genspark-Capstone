using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WarehouseFileArchiverAPI.Models.DTOs
{
    public class UserLoginResponse
    {
        public string Username { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public string? RefreshToken { get; set; }
        public string Role { get; set; } = string.Empty;
    }
}