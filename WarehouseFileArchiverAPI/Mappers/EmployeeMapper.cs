using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WarehouseFileArchiverAPI.Interfaces;
using WarehouseFileArchiverAPI.Models;
using WarehouseFileArchiverAPI.Models.DTOs;

namespace WarehouseFileArchiverAPI.Mappers
{
    public class EmployeeMapper 
    {
        public Employee MapEmployeeDtoToEmployee(EmployeeAddRequestDto employee)
        {
            var newEmployee = new Employee();
            newEmployee.ContactNumber = employee.ContactNumber;
            newEmployee.Email = employee.Email;
            newEmployee.FirstName = employee.FirstName;
            newEmployee.LastName = employee.LastName;
            newEmployee.IsActive = true;
            return newEmployee;
        }

        public Employee MapEmployeeUpdateDtoToEmployee(Employee employee,EmployeeUpdateRequestDto employeeDto)
        {
            employee.FirstName = employeeDto.FirstName;
            employee.LastName = employeeDto.LastName;
            employee.Email = employeeDto.Email;
            employee.ContactNumber = employeeDto.ContactNumber;
            return employee;
        }
    }
}