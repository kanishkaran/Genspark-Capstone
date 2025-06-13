using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WarehouseFileArchiverAPI.Contexts;
using WarehouseFileArchiverAPI.Exceptions;
using WarehouseFileArchiverAPI.Models;

namespace WarehouseFileArchiverAPI.Repositories
{
    public class EmployeeRepository : Repository<Guid, Employee>
    {

        public EmployeeRepository(WarehouseDBContext context) : base(context)
        {
            
        }
        public override async Task<IEnumerable<Employee>> GetAllAsync()
        {
            var employees = await _context.Employees.ToListAsync();
            if (employees.Count == 0)
                throw new CollectionEmptyException("There are no employees present in the DB");
            return employees;
        }

        public override async Task<Employee> GetByIdAsync(Guid id)
        {
            var employee = await _context.Employees.SingleOrDefaultAsync(emp => emp.Id == id);
            if (employee == null)
                throw new EmployeeNotFoundException($"Employee with Id: {id} is not found");

            return employee;
        }
    }
}