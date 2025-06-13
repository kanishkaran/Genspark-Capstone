using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.StaticAssets;

namespace WarehouseFileArchiverAPI.Models.DTOs
{
    public class ApiResponseDto<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public object? Errors { get; set; } = null;

        public static ApiResponseDto<T> SuccessReponse(string message, T data)
        {
            return new ApiResponseDto<T> { Success = true, Data = data, Message = message };
        }
        
        public static ApiResponseDto<T> ErrorResponse(string message, object errors)
        {
           return new ApiResponseDto<T> { Success = false, Data = default, Message = message, Errors = errors };
        }
    }
}