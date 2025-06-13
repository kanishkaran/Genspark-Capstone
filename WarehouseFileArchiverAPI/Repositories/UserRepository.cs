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
    public class UserRepository : Repository<string, User>
    {
        public UserRepository(WarehouseDBContext context) : base(context)
        {
            
        }
        public override async Task<IEnumerable<User>> GetAllAsync()
        {
            var users = await _context.Users.Include(u => u.Role)
                                            .ToListAsync();
            if (users.Count == 0)
                return new List<User>();
            return users.OrderBy(u => u.Username);
        }

        public override async Task<User> GetByIdAsync(string key)
        {
            var user = await _context.Users.SingleOrDefaultAsync(us => us.Username == key);
            
            return user;
        }
    }
}