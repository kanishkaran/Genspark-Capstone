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
    public class CategoryRepository : Repository<Guid, Category>
    {
        public CategoryRepository(WarehouseDBContext context) : base(context)
        {
            
        }
        public override async Task<IEnumerable<Category>> GetAllAsync()
        {
            var categories = await _context.Categories.ToListAsync();
            if (categories.Count == 0)
                return new List<Category>();
            return categories;
        }

        public override async Task<Category> GetByIdAsync(Guid id)
        {
            var category = await _context.Categories.SingleOrDefaultAsync(cat => cat.Id == id) ?? throw new CategoryNotFoundException($"Category with Id: {id} was not Found");
            return category;
        }
    }
}