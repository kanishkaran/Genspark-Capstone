
using WarehouseFileArchiverAPI.Interfaces;

namespace WarehouseFileArchiverAPI.Services
{
    public class EncryptionService : IEncryptionService
    {
        public string EncryptPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool VerifyPassword(string password, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
    }
}