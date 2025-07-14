using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WarehouseFileArchiverAPI.Exceptions;
using WarehouseFileArchiverAPI.Interfaces;
using WarehouseFileArchiverAPI.Models.DTOs;

namespace WarehouseFileArchiverAPI.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthenticationController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<ApiResponseDto<object>>> LoginUser(UserLoginRequest request)
        {
            try
            {
                var result = await _authenticationService.Login(request);

                return ApiResponseDto<object>.SuccessReponse("User LoggedIn Successfully", result);
            }
            catch (Exception e)
            {

                return BadRequest(ApiResponseDto<object>.ErrorResponse("Cannot Login User", new
                {
                    fields = e.Message
                }));
            }
        }
        [HttpPost("refresh")]
        [Authorize]
        public async Task<ActionResult<UserLoginResponse>> RefreshToken([FromBody] RefreshTokenRequestDto dto)
        {
            try
            {
                var response = await _authenticationService.RefreshToken(dto.Username, dto.RefreshToken);
                return Ok(response);
            }
            catch (Exception e)
            {

                return BadRequest(ApiResponseDto<object>.ErrorResponse("Cannot refresh token", new
                {
                    fields = e.Message
                }));
            }
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<ActionResult<ApiResponseDto<object>>> GetCurrentUser()
        {
            try
            {
                var username = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var role = User.FindFirstValue(ClaimTypes.Role);
                if (username == null || role == null)
                    throw new UserNotFoundException("No user found");
                var response = new CurrentUserDto
                {
                    UserName = username,
                    Role = role
                };

                return Ok(ApiResponseDto<CurrentUserDto>.SuccessReponse("User Fetched Successfully", response));
            }
            catch (Exception e)
            {

                return BadRequest(ApiResponseDto<object>.ErrorResponse("Cannot find User", new
                {
                    fields = e.Message
                }));
            }
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<ActionResult<ApiResponseDto<object>>> Logout()
        {
            try
            {
                var username = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var accessToken = HttpContext.Items["access_token"] as string;
                var result = await _authenticationService.Logout(username, accessToken);

                return Ok(ApiResponseDto<object>.SuccessReponse("User logged out successfully", result));
            }
            catch (Exception e)
            {
                return BadRequest(ApiResponseDto<object>.ErrorResponse("Cannot logout user", new
                {
                    fields = e.Message
                }));
            }
        }
    }
}