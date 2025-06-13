
using System.Text.Json;
using WarehouseFileArchiverAPI.Exceptions;
using WarehouseFileArchiverAPI.Interfaces;
using WarehouseFileArchiverAPI.Mappers;
using WarehouseFileArchiverAPI.Models;
using WarehouseFileArchiverAPI.Models.DTOs;

namespace WarehouseFileArchiverAPI.Services
{
    public class UserService : IUserService
    {
        private readonly IRepository<string, User> _userRepository;
        private readonly IEncryptionService _encryptionService;
        private readonly IRepository<Guid, Role> _roleRepository;
        private readonly IConfiguration _configuration;
        private readonly IAuditLogService _auditLogService;
        private readonly UserMapper _mapper;

        public UserService(IRepository<string, User> userRepository,
                            IRepository<Guid, Role> roleRepository,
                           IEncryptionService encryptionService,
                           IConfiguration configuration,
                           IAuditLogService auditLogService)
        {
            _userRepository = userRepository;
            _encryptionService = encryptionService;
            _roleRepository = roleRepository;
            _configuration = configuration;
            _auditLogService = auditLogService;
            _mapper = new();

        }
        public async Task<PaginationDto<UserListDto>> SearchUsers(SearchQueryDto searchDto)
        {
            var users = await _userRepository.GetAllAsync() ?? throw new CollectionEmptyException("No users in the Database");

            if (!searchDto.IncludeInactive)
                users = users.Where(u => !u.IsDeleted);

            if (!string.IsNullOrWhiteSpace(searchDto.Search))
            {
                var search = searchDto.Search.ToLower();
                users = users.Where(u =>
                    u.Username.Contains(search, StringComparison.CurrentCultureIgnoreCase) ||
                    (u.Role != null && u.Role.RoleName.Contains(search, StringComparison.CurrentCultureIgnoreCase))
                );
            }

            users = searchDto.SortBy?.ToLower() switch
            {
                "username" => searchDto.Desc ? users.OrderByDescending(u => u.Username) : users.OrderBy(u => u.Username),
                "rolename" => searchDto.Desc ? users.OrderByDescending(u => u.Role?.RoleName) : users.OrderBy(u => u.Role?.RoleName),
                _ => searchDto.Desc ? users.OrderByDescending(u => u.Username) : users.OrderBy(u => u.Username)
            };

            var totalRecords = users.Count();
            if (totalRecords == 0)
                throw new CollectionEmptyException("No User Matched");

            var items = users
                .Skip((searchDto.Page - 1) * searchDto.PageSize)
                .Take(searchDto.PageSize)
                .Select(MapToUserListDto)
                .ToList();

            return new PaginationDto<UserListDto>
            {
                Data = items,
                TotalRecords = totalRecords,
                Page = searchDto.Page,
                PageSize = searchDto.PageSize,
                TotalPages = (int)Math.Ceiling(totalRecords / (double)searchDto.PageSize)
            };
        }
        private UserListDto MapToUserListDto(User u)
        {
            return new UserListDto
            {
                Username = u.Username,
                RoleName = u.Role?.RoleName,
                IsDeleted = u.IsDeleted,
            };
        }

        public async Task<UserListDto> GetById(string username)
        {
            var user = await _userRepository.GetByIdAsync(username);
            if (user == null || user.IsDeleted)
                throw new CollectionEmptyException($"User with username: {username} was not found or is deleted");
            var role = await _roleRepository.GetByIdAsync(user.RoleId);
            user.Role = role;
            return MapToUserListDto(user);
        }

        public async Task<UserListDto> AddUser(UserAddRequestDto userDto, string currUser)
        {
            if (userDto.IsAdmin)
            {
                if (string.IsNullOrWhiteSpace(userDto.SecretKey) || userDto.SecretKey != _configuration["Keys:AdminAuthKey"])
                    throw new UnauthorizedAccessException("Invalid or missing admin secret key.");
            }

            var existing = await _userRepository.GetByIdAsync(userDto.Username);
            if (existing != null && !existing.IsDeleted)
                throw new Exception($"User with username {userDto.Username} already exists.");


            var roles = await _roleRepository.GetAllAsync();
            var role = roles.FirstOrDefault(r => r.RoleName.ToLower() == userDto.Role.ToLower());
            if (role == null)
                throw new RoleNotFoundException($"Role: '{userDto.Role}' does not exist.");

            var passwordHash = _encryptionService.EncryptPassword(userDto.Password);

            var user = _mapper.MapAddReqDtoToUser(userDto);
            user.PasswordHash = passwordHash;
            user.RoleId = role.Id;

            var added = await _userRepository.AddAsync(user);
            await _auditLogService.LogAsync(
            "User",
            Guid.TryParse(added.Username, out var userId) ? userId : Guid.NewGuid(),
            "Add",
            currUser ?? "system",
            JsonSerializer.Serialize(userDto)
        );
            return MapToUserListDto(added);
        }

        public async Task<UserListDto> UpdateUserRole(string username, string newRole)
        {
            if (newRole == "Admin")
            {
                throw new UnauthorizedAccessException("Admin role cannot be assigned ");
            }
            var user = await _userRepository.GetByIdAsync(username);
            if (user == null || user.IsDeleted)
                throw new CollectionEmptyException($"User with username: {username} was not found or is deleted");

            var roles = await _roleRepository.GetAllAsync();
            var role = roles.FirstOrDefault(r => r.RoleName.Equals(newRole, StringComparison.OrdinalIgnoreCase));
            if (role == null)
                throw new RoleNotFoundException($"Role '{newRole}' does not exist.");

            user.RoleId = role.Id;
            var updated = await _userRepository.UpdateAsync(username, user);
            await _auditLogService.LogAsync(
            "User",
            Guid.TryParse(user.Username, out var userId) ? userId : Guid.NewGuid(),
            "UpdateRole",
            "Admin",
            $"$user: {username} role: {newRole}"
        );
            user.Role = role;
            return MapToUserListDto(updated);
        }

        public async Task<UserListDto> ChangeUserPassword(string username, string oldPassword, string newPassword, string currUser)
        {
            var user = await _userRepository.GetByIdAsync(username);
            if (user == null || user.IsDeleted)
                throw new CollectionEmptyException($"User with username: {username} was not found or is deleted");

            if (user.Username != currUser)
                throw new UnauthorizedAccessException("You are not authorised to change password");

            if (!_encryptionService.VerifyPassword(oldPassword, user.PasswordHash))
                throw new Exception("Old password is incorrect.");

            user.PasswordHash = _encryptionService.EncryptPassword(newPassword);
            var updated = await _userRepository.UpdateAsync(username, user);
            await _auditLogService.LogAsync(
            "User",
            Guid.TryParse(user.Username, out var userId) ? userId : Guid.NewGuid(),
            "ChangePassword",
            currUser,
            $"user: {username} changed password"
        );
            return MapToUserListDto(updated);
        }

        public async Task<string> DeleteUser(string username, string currUser)
        {
            var user = await _userRepository.GetByIdAsync(username);
            if (user == null || user.IsDeleted)
                throw new CollectionEmptyException($"User with username: {username} was not found or is already deleted");

            user.IsDeleted = true;
            await _userRepository.UpdateAsync(username, user);

            await _auditLogService.LogAsync(
            "User",
            Guid.TryParse(user.Username, out var userId) ? userId : Guid.NewGuid(),
            "Delete",
            currUser,
            $"User with username {username} marked as deleted."
        );


            return $"User with username {username} deleted.";
        }
    }
}