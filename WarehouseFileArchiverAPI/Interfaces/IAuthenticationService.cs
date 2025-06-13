using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WarehouseFileArchiverAPI.Models.DTOs;

namespace WarehouseFileArchiverAPI.Interfaces
{
    public interface IAuthenticationService
    {
        Task<UserLoginResponse> Login(UserLoginRequest user);
        Task<UserLoginResponse> RefreshToken(string username, string refreshToken);
        Task<string> Logout(string username, string accessToken);
    }
}