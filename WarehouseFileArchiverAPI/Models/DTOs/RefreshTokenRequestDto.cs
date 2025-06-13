using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WarehouseFileArchiverAPI.Models.DTOs
{
public class RefreshTokenRequestDto
{
        public string Username { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
}
}