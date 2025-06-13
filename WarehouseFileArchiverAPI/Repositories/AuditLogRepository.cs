using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WarehouseFileArchiverAPI.Contexts;
using WarehouseFileArchiverAPI.Models;

namespace WarehouseFileArchiverAPI.Repositories
{
    public class AuditLogRepository : Repository<Guid, AuditLog>
    {
        public AuditLogRepository(WarehouseDBContext context) : base(context)
        {
        }

        public override async Task<IEnumerable<AuditLog>> GetAllAsync()
        {
            return await _context.AuditLogs.ToListAsync();
        }

        public override async Task<AuditLog> GetByIdAsync(Guid id)
        {
            return await _context.AuditLogs.FirstOrDefaultAsync(a => a.Id == id)
                ?? throw new Exception("AuditLog not found");
        }
    }
}