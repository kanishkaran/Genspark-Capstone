using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using WarehouseFileArchiverAPI.Exceptions;
using WarehouseFileArchiverAPI.Interfaces;
using WarehouseFileArchiverAPI.Models;

namespace WarehouseFileArchiverAPI.Services
{
    public class TokenService : ITokenService
    {
        private readonly SymmetricSecurityKey _securityKey;
        private readonly IRepository<string, User> _userRepository;
        private readonly IRepository<Guid, Role> _roleRepository;
        private readonly IRepository<Guid, AccessLevel> _accessLevelRepository;

        public TokenService(IConfiguration configuration,
                            IRepository<string, User> userRepository,
                            IRepository<Guid, Role> roleRepository,
                            IRepository<Guid, AccessLevel> accessRepository)
        {
            _securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Keys:JwtTokenKey"]));
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _accessLevelRepository = accessRepository;
        }

        public async Task<string> GenerateRefreshToken(User user)
        {

            var randomBytes = new byte[64];
            using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
                return Convert.ToBase64String(randomBytes);
            }
        }

        public async Task<string> GenerateToken(User user)
        {
            var currUser = await _userRepository.GetByIdAsync(user.Username);

            var role = await _roleRepository.GetByIdAsync(currUser.RoleId);

            var access = await _accessLevelRepository.GetByIdAsync(role.AccessLevelId);

            List<Claim> claims =
            [
                new(ClaimTypes.NameIdentifier, user.Username),
                new(ClaimTypes.Role, role.RoleName),
                new("AccessLevel", access.Access)
            ];

            var credential = new SigningCredentials(_securityKey, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = credential
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

       
    }
}