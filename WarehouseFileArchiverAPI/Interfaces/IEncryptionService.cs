

namespace WarehouseFileArchiverAPI.Interfaces
{
    public interface IEncryptionService
    {
        string EncryptPassword(string password);

        bool VerifyPassword(string password, string hash);
    }
}