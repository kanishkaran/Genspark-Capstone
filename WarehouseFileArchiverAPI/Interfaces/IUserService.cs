using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WarehouseFileArchiverAPI.Models.DTOs;

namespace WarehouseFileArchiverAPI.Interfaces
{
    public interface IUserService
    {
        Task<PaginationDto<UserListDto>> SearchUsers(SearchQueryDto searchDto);
        Task<UserListDto> GetById(string username);
        Task<UserListDto> AddUser(UserAddRequestDto userDto, string currUser);
        Task<UserListDto> UpdateUserRole(string username, string newRole);
        Task<UserListDto> ChangeUserPassword(string username, string oldPassword, string newPassword, string currUser);
        Task<string> DeleteUser(string username, string currUser);
    }
}