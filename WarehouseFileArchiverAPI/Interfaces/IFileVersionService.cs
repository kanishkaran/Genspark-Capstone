using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WarehouseFileArchiverAPI.Models;
using WarehouseFileArchiverAPI.Models.DTOs;

namespace WarehouseFileArchiverAPI.Interfaces
{
    public interface IFileVersionService
    {
        Task<FileVersion?> GetFileVersionByArchiveId(Guid id);
        Task<bool> ValidateChecksum(string checksum, Guid archiveId);
        Task<FileVersion> GetFileVersionByVersionNumber(Guid archiveId, int versionNumber);
        Task<ICollection<FileVersionListDto>> GetAllVersions();
        Task<ICollection<FileVersionListDto>> GetFileVersionsByArchiveId(Guid id);
    }
}