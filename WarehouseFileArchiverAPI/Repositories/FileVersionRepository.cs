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
    public class FileVersionRepository : Repository<Guid, FileVersion>
    {
        public FileVersionRepository(WarehouseDBContext context) : base(context)
        {
            
        }
        public override async Task<IEnumerable<FileVersion>> GetAllAsync()
        {
            var fileVersions = await _context.FileVersions.ToListAsync();
            
            return fileVersions.OrderBy(fv => fv.CreatedAt);
        }

        public override async Task<FileVersion> GetByIdAsync(Guid id)
        {
            var fileVersion = await _context.FileVersions.SingleOrDefaultAsync(fv => fv.Id == id) ?? throw new VersionMismatchException($"file version with id: {id} was not found");
            return fileVersion;
        }
    }
}