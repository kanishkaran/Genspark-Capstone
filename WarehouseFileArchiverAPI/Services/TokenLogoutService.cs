using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WarehouseFileArchiverAPI.Interfaces;

namespace WarehouseFileArchiverAPI.Services
{
    public class TokenLogoutService : ITokenLogoutService
    {
        private readonly ConcurrentDictionary<string, DateTime> _logoutList = new();
        public bool IsLoggedOut(string token)
        {
            if (_logoutList.TryGetValue(token, out var expiry))
            {
                if (DateTime.UtcNow < expiry)
                    return true;
                _logoutList.TryRemove(token, out _);
            }
            return false;
        }

        public void LogoutToken(string token, DateTime expiry)
        {
            if (!string.IsNullOrEmpty(token))
                _logoutList[token] = expiry;
        }
    }
}