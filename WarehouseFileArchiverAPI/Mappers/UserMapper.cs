using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using WarehouseFileArchiverAPI.Interfaces;
using WarehouseFileArchiverAPI.Models;
using WarehouseFileArchiverAPI.Models.DTOs;

namespace WarehouseFileArchiverAPI.Mappers
{
    public class UserMapper 
    {
        public User MapEmployeeAddDtoToUser(EmployeeAddRequestDto employee)
        {
            User newUser = new();
            newUser.Username = employee.Email;
            newUser.IsDeleted = false;
            return newUser;
        }

        public User MapAddReqDtoToUser(UserAddRequestDto user)
        {
            User newUser = new();
            newUser.Username = user.Username;
            newUser.IsDeleted = false;
            return newUser;
        }
    }
}