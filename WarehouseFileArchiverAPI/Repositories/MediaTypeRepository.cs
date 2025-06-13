

using Microsoft.EntityFrameworkCore;
using WarehouseFileArchiverAPI.Contexts;
using WarehouseFileArchiverAPI.Exceptions;
using WarehouseFileArchiverAPI.Models;

namespace WarehouseFileArchiverAPI.Repositories
{
    public class MediaTypeRepository : Repository<Guid, MediaType>
    {
        public MediaTypeRepository(WarehouseDBContext context) : base(context)
        {
            
        }
        public override async Task<IEnumerable<MediaType>> GetAllAsync()
        {
            var mediaTypes = await _context.MediaTypes.ToListAsync();
            if (mediaTypes.Count == 0)
            {
                return new List<MediaType>();
            }
            return mediaTypes.OrderBy(md => md.Extension);
        }

        public override async Task<MediaType> GetByIdAsync(Guid id)
        {
            var mediaType = await _context.MediaTypes.SingleOrDefaultAsync(mt => mt.Id == id) ?? throw new InvalidMediaException($"The media type with id: {id} was not found / not supported");
            return mediaType;
        }
    }
}