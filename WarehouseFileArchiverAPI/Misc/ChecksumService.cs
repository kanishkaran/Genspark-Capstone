using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using WarehouseFileArchiverAPI.Interfaces;

namespace WarehouseFileArchiverAPI.Misc
{
    public class ChecksumService : IChecksumService
    {
        public async Task<string> ComputeMD5(Stream data)
        {
            var hash = await MD5.HashDataAsync(data);
            return ConvertToHex(hash);
        }

        public async Task<string> ComputeSHA256(Stream data)
        {
            var hash = await SHA256.HashDataAsync(data);
            return ConvertToHex(hash);
        }

        public string ConvertToHex(byte[] hash)
        {
            var sb = new StringBuilder();
            foreach (var b in hash)
            {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }
    }
}