using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WarehouseFileArchiverAPI.Interfaces;
using WarehouseFileArchiverAPI.Models;

namespace WarehouseFileArchiverAPI.Services
{
    public class FileVersionService : IFileVersionService
    {
        private readonly IRepository<Guid, FileVersion> _fileVersionRepository;

        public FileVersionService(IRepository<Guid, FileVersion> fileVersionRepository)
        {
            _fileVersionRepository = fileVersionRepository;
        }
        public async Task<FileVersion?> GetFileVersionByArchiveId(Guid id)
        {
            var fileVersions = await _fileVersionRepository.GetAllAsync();
            var version = fileVersions.Where(fv => fv.FileArchiveId == id).OrderByDescending(fv => fv.VersionNumber).FirstOrDefault();
            return version;
        }

        public async Task<FileVersion> GetFileVersionByVersionNumber(Guid archiveId, int versionNumber)
        {
            var fileVersions = await _fileVersionRepository.GetAllAsync();
            return fileVersions.FirstOrDefault(fv => fv.FileArchiveId == archiveId && fv.VersionNumber == versionNumber);
        }

        public async Task<bool> ValidateChecksum(string checksum, Guid archiveId)
        {
            var fileVersions = await _fileVersionRepository.GetAllAsync();
            var existingVersion = fileVersions.FirstOrDefault(fv => fv.Checksum == checksum && fv.FileArchiveId == archiveId);
            return existingVersion == null;
        }
    }
}