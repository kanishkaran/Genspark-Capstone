using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WarehouseFileArchiverAPI.Models;

namespace WarehouseFileArchiverAPI.Interfaces
{
    public interface IFileVersionService
    {
        Task<FileVersion?> GetFileVersionByArchiveId(Guid id);

        Task<bool> ValidateChecksum(string checksum, Guid archiveId);

        Task<FileVersion> GetFileVersionByVersionNumber(Guid archiveId, int versionNumber);
    }
}