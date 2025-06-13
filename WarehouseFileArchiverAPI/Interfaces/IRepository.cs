using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WarehouseFileArchiverAPI.Interfaces
{
    public interface IRepository<K, T> where T : class
    {
        Task<T> GetByIdAsync(K id);

        Task<T> AddAsync(T item);

        Task<T> UpdateAsync(K id,T item);

        Task<IEnumerable<T>> GetAllAsync();
    }
}