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
    public class AccessLevelRepository : Repository<Guid, AccessLevel>
    {
        public AccessLevelRepository(WarehouseDBContext context) : base(context)
        {
            
        }
        public override async Task<IEnumerable<AccessLevel>> GetAllAsync()
        {
            var accessLevels = await _context.AccessLevels.ToListAsync();
            if (accessLevels.Count == 0)
                return new List<AccessLevel>();

            return accessLevels;
        }

        public override async Task<AccessLevel> GetByIdAsync(Guid id)
        {
            var accessLevel = await _context.AccessLevels.SingleOrDefaultAsync(al => al.Id == id) ?? throw new InvalidAccessLevelException($"The access level with id: {id} was not found");
            return accessLevel;
        }
    }
}