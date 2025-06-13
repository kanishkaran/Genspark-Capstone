using System;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using WarehouseFileArchiverAPI.Exceptions;
using WarehouseFileArchiverAPI.Interfaces;
using WarehouseFileArchiverAPI.Models;
using WarehouseFileArchiverAPI.Models.DTOs;

namespace WarehouseFileArchiverAPI.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IRepository<string, User> _userRepository;
        private readonly IEncryptionService _encryptionService;
        private readonly ITokenService _tokenService;
        private readonly ITokenLogoutService _tokenLogoutService;

        public AuthenticationService(IRepository<string, User> userRepository,
                                     IEncryptionService encryptionService,
                                     ITokenService tokenService,
                                     ITokenLogoutService tokenLogoutService)
        {
            _userRepository = userRepository;
            _encryptionService = encryptionService;
            _tokenService = tokenService;
            _tokenLogoutService = tokenLogoutService;
        }

        public async Task<UserLoginResponse> Login(UserLoginRequest user)
        {
            try
            {
                var currUser = await _userRepository.GetByIdAsync(user.Username);
                if(currUser == null || currUser.IsDeleted)
                    throw new UserNotFoundException($"There is no such user {user.Username} or user is deleted");
                var isValid = _encryptionService.VerifyPassword(user.Password, currUser.PasswordHash);

                if (!isValid)
                    throw new Exception("Invalid Credentials");
                

                var token = await _tokenService.GenerateToken(currUser);
                var refreshToken = await _tokenService.GenerateRefreshToken(currUser);

                currUser.RefreshToken = refreshToken;
                currUser.RefreshTokenExpiry = DateTime.UtcNow.AddDays(5);
                await _userRepository.UpdateAsync(currUser.Username, currUser);

                return new UserLoginResponse
                {
                    Username = currUser.Username,
                    Token = token,
                    RefreshToken = refreshToken
                };
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<UserLoginResponse> RefreshToken(string username, string refreshToken)
        {
            var user = await _userRepository.GetByIdAsync(username);
            if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiry < DateTime.UtcNow || user.IsDeleted)
                throw new Exception("Invalid or expired refresh token.");

            var newToken = await _tokenService.GenerateToken(user);
            var newRefreshToken = await _tokenService.GenerateRefreshToken(user);

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
            await _userRepository.UpdateAsync(user.Username, user);

            return new UserLoginResponse
            {
                Username = user.Username,
                Token = newToken,
                RefreshToken = newRefreshToken
            };
        }

        public async Task<string> Logout(string username, string accessToken)
        {
            var user = await _userRepository.GetByIdAsync(username);
            if (user == null || user.IsDeleted)
                throw new CollectionEmptyException($"User with username: {username} was not found or is deleted");

            user.RefreshToken = null;
            user.RefreshTokenExpiry = null;
            await _userRepository.UpdateAsync(username, user);

            var tokenExpiry = GetTokenExpiry(accessToken);
            _tokenLogoutService.LogoutToken(accessToken, tokenExpiry);

            return $"User with username {username} logged out successfully.";
        }

        private DateTime GetTokenExpiry(string accessToken)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.ReadJwtToken(accessToken).ValidTo;
        }
    }
}