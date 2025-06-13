using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using WarehouseFileArchiverAPI.Exceptions;
using WarehouseFileArchiverAPI.Interfaces;
using WarehouseFileArchiverAPI.Mappers;
using WarehouseFileArchiverAPI.Models;
using WarehouseFileArchiverAPI.Models.DTOs;

namespace WarehouseFileArchiverAPI.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IRepository<string, User> _userRepository;
        private readonly IRepository<Guid, Employee> _employeeRepository;
        private readonly IEncryptionService _encryptionService;
        private readonly IRepository<Guid, Role> _roleRepository;
        private readonly EmployeeMapper _employeeMapper;
        private readonly UserMapper _userMapper;
        private readonly IAuditLogService _auditLogService;

        public EmployeeService(IRepository<string, User> userRepository,
                               IRepository<Guid, Employee> employeeRepository,
                               IEncryptionService encryptionService,
                               IRepository<Guid, Role> roleRepository,
                               IAuditLogService auditLogService)
        {
            _userRepository = userRepository;
            _employeeRepository = employeeRepository;
            _encryptionService = encryptionService;
            _roleRepository = roleRepository;
            _employeeMapper = new();
            _userMapper = new();
            _auditLogService = auditLogService;
        }

        public async Task<Employee> GetEmployeeByEmail(string email)
        {
            try
            {
                var employees = await _employeeRepository.GetAllAsync();

                if (employees.ToList().Count == 0)
                {
                    throw new CollectionEmptyException("There are no employees in the database");
                }

                return employees.FirstOrDefault(ep => ep.Email == email && ep.IsActive) ?? throw new EmployeeNotFoundException($"Employee with email: {email} was not found or deleted");
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Employee> RegisterEmployee(EmployeeAddRequestDto employee)
        {
            try
            {

                var roles = await _roleRepository.GetAllAsync();
                var defaultRole = roles.FirstOrDefault(r => r.RoleName.ToLower() == "staff" && !r.IsDeleted)
                                  ?? throw new RoleNotFoundException("Default role 'staff' not found.");

                var users = await _userRepository.GetAllAsync();
                var user = users.FirstOrDefault(u => u.Username == employee.Email && !u.IsDeleted);

                var employees = await _employeeRepository.GetAllAsync();
                var existingEmployee = employees.FirstOrDefault(e => e.Email == employee.Email && e.IsActive);

                if (existingEmployee != null)
                {
                    if (existingEmployee.IsActive)
                        throw new UserAlreadyExistException($"Employee with email {employee.Email} already exists.");

                    throw new UnauthorizedAccessException("Trying is to add deleted Emloyees is restricted");
                }


                if (user == null)
                {
                    var passwordHash = _encryptionService.EncryptPassword(employee.Password);
                    user = _userMapper.MapEmployeeAddDtoToUser(employee);
                    user.PasswordHash = passwordHash;
                    user.RoleId = defaultRole.Id;
                    await _userRepository.AddAsync(user);
                }

                var newEmployee = _employeeMapper.MapEmployeeDtoToEmployee(employee);
                newEmployee.User = user;
                var addedEmployee = await _employeeRepository.AddAsync(newEmployee);

                await _auditLogService.LogAsync(
                    "Employee",
                    addedEmployee.Id,
                    "Register",
                    employee.Email,
                    JsonSerializer.Serialize(employee)
                );

                return addedEmployee;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<PaginationDto<Employee>> SearchEmployee(EmployeeSearchDto searchDto)
        {
            var employees = await _employeeRepository.GetAllAsync() ?? throw new CollectionEmptyException("No employees in the Database");
            if (!searchDto.IncludeInactive)
                employees = employees.Where(e => e.IsActive);

            employees = ApplyEmployeeFilters(employees, searchDto);
            employees = ApplyEmployeeSorting(employees, searchDto);

            var totalRecords = employees.Count();
            if (totalRecords == 0)
                throw new CollectionEmptyException("No employees Matched ");

            var items = employees
                .Skip((searchDto.Page - 1) * searchDto.PageSize)
                .Take(searchDto.PageSize)
                .ToList();

            return new PaginationDto<Employee>
            {
                Data = items,
                TotalRecords = totalRecords,
                Page = searchDto.Page,
                PageSize = searchDto.PageSize,
                TotalPages = (int)Math.Ceiling(totalRecords / (double)searchDto.PageSize)
            };
        }

        private static IEnumerable<Employee> ApplyEmployeeFilters(IEnumerable<Employee> employees, EmployeeSearchDto searchDto)
        {
            if (!string.IsNullOrWhiteSpace(searchDto.FirstName))
                employees = employees.Where(e => e.FirstName.ToLower().Contains(searchDto.FirstName.ToLower()));

            if (!string.IsNullOrWhiteSpace(searchDto.LastName))
                employees = employees.Where(e => e.LastName.ToLower().Contains(searchDto.LastName.ToLower()));

            if (!string.IsNullOrWhiteSpace(searchDto.Email))
                employees = employees.Where(e => e.Email.ToLower().Contains(searchDto.Email.ToLower()));

            if (!string.IsNullOrWhiteSpace(searchDto.ContactNumber))
                employees = employees.Where(e => e.ContactNumber.ToLower().Contains(searchDto.ContactNumber.ToLower()));

            return employees;
        }

        private static IEnumerable<Employee> ApplyEmployeeSorting(IEnumerable<Employee> employees, EmployeeSearchDto searchDto)
        {
            return searchDto.SortBy?.ToLower() switch
            {
                "firstname" => searchDto.Desc ? employees.OrderByDescending(e => e.FirstName) : employees.OrderBy(e => e.FirstName),
                "lastname" => searchDto.Desc ? employees.OrderByDescending(e => e.LastName) : employees.OrderBy(e => e.LastName),
                "email" => searchDto.Desc ? employees.OrderByDescending(e => e.Email) : employees.OrderBy(e => e.Email),
                "contactnumber" => searchDto.Desc ? employees.OrderByDescending(e => e.ContactNumber) : employees.OrderBy(e => e.ContactNumber),
                _ => employees
            };
        }

        public async Task<Employee> GetById(Guid id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);
            if (employee == null || !employee.IsActive)
                throw new EmployeeNotFoundException($"Employee with id: {id} was not found or is deleted");
            return employee;
        }

        public async Task<Employee> UpdateEmployee(Guid id, EmployeeUpdateRequestDto employeeDto, string currUser)
        {

            var employee = await _employeeRepository.GetByIdAsync(id);
            if (!employee.IsActive)
                throw new EmployeeNotFoundException($"Employee with {employee.Email} is deleted / not active");

            if (employee.Email != currUser)
                throw new UnauthorizedAccessException($"Updating Details of {employee.Email} is not Authorised");


          
            employee = _employeeMapper.MapEmployeeUpdateDtoToEmployee(employeeDto);

            var updatedEmployee = await _employeeRepository.UpdateAsync(employee.Id, employee);

            await _auditLogService.LogAsync(
                "Employee",
                updatedEmployee.Id,
                "Update",
                currUser,
                $"Old: {employee}, New: {JsonSerializer.Serialize(employeeDto)}"
            );

            return updatedEmployee;
        }

        public async Task<string> DeleteEmployee(Guid id, string currUser)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);
            if (employee == null || !employee.IsActive)
                throw new EmployeeNotFoundException($"Employee with id: {id} was not found or is already deleted");

            employee.IsActive = false;
            await _employeeRepository.UpdateAsync(employee.Id, employee);

            await _auditLogService.LogAsync(
                "Employee",
                employee.Id,
                "Delete",
                currUser,
                $"Employee with id {id} marked as deleted."
            );

            return $"Employee with id {id} deleted.";
        }
    }
}