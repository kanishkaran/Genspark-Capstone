using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WarehouseFileArchiverAPI.Exceptions
{
    public class VersionMismatchException : Exception
    {
        private readonly string _message;

        public VersionMismatchException(string message)
        {
            _message = message;
        }

        public override string Message => _message;
    }
}