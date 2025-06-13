using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WarehouseFileArchiverAPI.Exceptions
{
    public class UserAlreadyExistException : Exception
    {
        public UserAlreadyExistException(string userName) : base($"The User with Username: {userName} already EXISTS")
        {
            
        }
    }
}