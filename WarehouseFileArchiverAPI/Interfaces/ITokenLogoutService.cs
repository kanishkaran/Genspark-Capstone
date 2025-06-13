using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WarehouseFileArchiverAPI.Interfaces
{
    public interface ITokenLogoutService
    {
        void LogoutToken(string token, DateTime expiry);
        bool IsLoggedOut(string token);
    }
}