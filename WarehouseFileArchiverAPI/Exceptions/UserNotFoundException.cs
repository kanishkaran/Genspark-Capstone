using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WarehouseFileArchiverAPI.Exceptions
{
    public class UserNotFoundException : Exception
    {
        private readonly string _user;

        public UserNotFoundException(string user)
        {
            _user = user;
        }

        public override string Message => _user;
        
    }
}