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
    public class FileArchiveRepository : Repository<Guid, FileArchive>
    {
        public FileArchiveRepository(WarehouseDBContext context) : base(context)
        {

        }
        public override async Task<IEnumerable<FileArchive>> GetAllAsync()
        {
            var fileArchives = await _context.FileArchives.Include(fa => fa.Category)
                                                            .Include(fa => fa.Employee)
                                                            .ToListAsync();
            return fileArchives;
        }

        public override async Task<FileArchive> GetByIdAsync(Guid id)
        {
            var fileArchive = await _context.FileArchives.SingleOrDefaultAsync(fa => fa.Id == id) ?? throw new FileArchiveNotFoundException($"File archive with Id: {id} was not found");
            return fileArchive;
        }
    }
}