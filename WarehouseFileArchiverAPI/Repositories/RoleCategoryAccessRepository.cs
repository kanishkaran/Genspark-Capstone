using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WarehouseFileArchiverAPI.Contexts;
using WarehouseFileArchiverAPI.Models;

namespace WarehouseFileArchiverAPI.Repositories
{
    public class RoleCategoryAccessRepository : Repository<Guid, RoleCategoryAccess>
    {

        public RoleCategoryAccessRepository(WarehouseDBContext context) : base(context)
        {

        }
        public override async Task<IEnumerable<RoleCategoryAccess>> GetAllAsync()
        {
            return await _context.RoleCategoryAccesses
                .Include(r => r.Role)
                .Include(r => r.Category)
                .ToListAsync();
        }

        public override async Task<RoleCategoryAccess> GetByIdAsync(Guid id)
        {
            var value = await _context.RoleCategoryAccesses.FirstOrDefaultAsync(r => r.Id == id) ?? throw new Exception("Category Access Not found");
            return value;
        }
    }
}