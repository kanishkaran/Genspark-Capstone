using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WarehouseFileArchiverAPI.Interfaces;
using WarehouseFileArchiverAPI.Models;
using WarehouseFileArchiverAPI.Models.DTOs;

namespace WarehouseFileArchiverAPI.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponseDto<object>>> SearchUsers([FromQuery] SearchQueryDto searchDto)
        {
            try
            {
                var result = await _userService.SearchUsers(searchDto);
                var response = ApiResponseDto<PaginationDto<UserListDto>>.SuccessReponse("Fetched users successfully", result);
                return Ok(response);
            }
            catch (Exception e)
            {

                return BadRequest(ApiResponseDto<object>.ErrorResponse("Users not found", new
                {
                    fields = e.Message
                }));
            }
        }

        [HttpGet("{username}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponseDto<object>>> GetById(string username)
        {
            try
            {
                var user = await _userService.GetById(username);
                return Ok(ApiResponseDto<UserListDto>.SuccessReponse("Fetched user successfully", user));
            }
            catch (Exception e)
            {
                return NotFound(ApiResponseDto<object>.ErrorResponse("User not found", new
                {
                    fields = e.Message
                }));
            }
        }

        [HttpPost]
       [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponseDto<object>>> AddUser(UserAddRequestDto userDto)
        {
            try
            {
                var user = await _userService.AddUser(userDto, User.FindFirstValue(ClaimTypes.NameIdentifier));
                return Ok(ApiResponseDto<UserListDto>.SuccessReponse("User added successfully", user));
            }
            catch (Exception e)
            {
                return BadRequest(ApiResponseDto<object>.ErrorResponse("Cannot add user", new
                {
                    fields = e.Message
                }));
            }
        }

        [HttpPut("role")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponseDto<object>>> UpdateUserRole(UserRoleUpdateRequestDto requestDto)
        {
            try
            {   
                var user = await _userService.UpdateUserRole(requestDto.Username, requestDto.Role);
                return Ok(ApiResponseDto<UserListDto>.SuccessReponse("User role updated successfully", user));
            }
            catch (Exception e)
            {
                return BadRequest(ApiResponseDto<object>.ErrorResponse("Cannot update user role", new
                {
                    fields = e.Message
                }));
            }
        }

        [HttpPut("{username}/password")]
        [Authorize]
        public async Task<ActionResult<ApiResponseDto<object>>> ChangePassword(string username, [FromBody] UserPasswordChangeDto dto)
        {
            try
            {
                var user = await _userService.ChangeUserPassword(username, dto.OldPassword, dto.NewPassword, User.FindFirstValue(ClaimTypes.NameIdentifier));
                return Ok(ApiResponseDto<UserListDto>.SuccessReponse("Password changed successfully", user));
            }
            catch (Exception e)
            {
                return BadRequest(ApiResponseDto<object>.ErrorResponse("Cannot change password", new
                {
                    fields = e.Message
                }));
            }
        }

        [HttpDelete("{username}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponseDto<object>>> DeleteUser(string username)
        {
            try
            {
                var message = await _userService.DeleteUser(username, User.FindFirstValue(ClaimTypes.NameIdentifier));
                return Ok(ApiResponseDto<object>.SuccessReponse("Delete Successful", message));
            }
            catch (Exception e)
            {
                return NotFound(ApiResponseDto<object>.ErrorResponse("Cannot delete user", new
                {
                    fields = e.Message
                }));
            }
        }
    }
}