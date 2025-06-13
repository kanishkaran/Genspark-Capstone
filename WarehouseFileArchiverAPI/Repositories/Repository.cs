using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WarehouseFileArchiverAPI.Contexts;
using WarehouseFileArchiverAPI.Interfaces;

namespace WarehouseFileArchiverAPI.Repositories
{
    public abstract class Repository<K, T> : IRepository<K, T> where T : class
    {
        protected readonly WarehouseDBContext _context;

        public Repository(WarehouseDBContext context)
        {
            _context = context;
        }

        public abstract Task<T> GetByIdAsync(K id);
        public abstract Task<IEnumerable<T>> GetAllAsync();
        public async Task<T> AddAsync(T item)
        {
            _context.Add(item);
            await _context.SaveChangesAsync();
            return item;
        }


        public async Task<T> UpdateAsync(K id, T item)
        {
            var oldItem = await GetByIdAsync(id);
            _context.Entry(oldItem).CurrentValues.SetValues(item);

            await _context.SaveChangesAsync();
            return item;
        }
    }
}