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
    public class RoleRepository : Repository<Guid, Role>
    {
        public RoleRepository(WarehouseDBContext context) : base(context)
        {
            
        }
        public override async Task<IEnumerable<Role>> GetAllAsync()
        {
            var roles = await _context.Roles.Include(r => r.AccessLevel)
                                            .ToListAsync();
            if (roles == null)
                return new List<Role>();
            return roles.OrderBy(r => r.RoleName);
        }

        public override async Task<Role> GetByIdAsync(Guid id)
        {
            var role = await _context.Roles.SingleOrDefaultAsync(role => role.Id == id) ?? throw new RoleNotFoundException($"Employee with id: {id} not found");
            return role;
        }
    }
}