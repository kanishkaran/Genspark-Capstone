using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WarehouseFileArchiverAPI.Models;
using WarehouseFileArchiverAPI.Models.DTOs;

namespace WarehouseFileArchiverAPI.Interfaces
{
    public interface IEmployeeService
    {

        Task<PaginationDto<Employee>> SearchEmployee(EmployeeSearchDto searchDto);
        Task<Employee> GetById(Guid id);
        Task<Employee> RegisterEmployee(EmployeeAddRequestDto employee);
        Task<Employee> GetEmployeeByEmail(string email);
        Task<Employee> UpdateEmployee(Guid id, EmployeeUpdateRequestDto employee, string currUser, string role);
        Task<string> DeleteEmployee(Guid id, string currUser);
    }
}