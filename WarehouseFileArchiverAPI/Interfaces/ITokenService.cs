using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WarehouseFileArchiverAPI.Models;

namespace WarehouseFileArchiverAPI.Interfaces
{
    public interface ITokenService
    {
        Task<string> GenerateToken(User user);
        Task<string> GenerateRefreshToken(User user);
    }
}