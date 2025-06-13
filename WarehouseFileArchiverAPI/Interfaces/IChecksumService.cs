using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WarehouseFileArchiverAPI.Interfaces
{
    public interface IChecksumService
    {
        Task<string> ComputeMD5(Stream data);
        Task<string> ComputeSHA256(Stream data);
        string ConvertToHex(byte[] hash);
    }
}