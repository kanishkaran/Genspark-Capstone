using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WarehouseFileArchiverAPI.Exceptions;
using WarehouseFileArchiverAPI.Interfaces;
using WarehouseFileArchiverAPI.Models;
using WarehouseFileArchiverAPI.Models.DTOs;

namespace WarehouseFileArchiverAPI.Services
{
    public class FileVersionService : IFileVersionService
    {
        private readonly IRepository<Guid, FileVersion> _fileVersionRepository;
        private readonly ILogger<FileVersionService> _logger;

        public FileVersionService(IRepository<Guid, FileVersion> fileVersionRepository, ILogger<FileVersionService> logger)
        {
            _fileVersionRepository = fileVersionRepository;
            _logger = logger;
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

        public async Task<ICollection<FileVersionListDto>> GetAllVersions()
        {
            var fileVersions = await _fileVersionRepository.GetAllAsync() ?? throw new CollectionEmptyException("There are no file versions in the database");


            var items = fileVersions
                        .Select(MapFileVersionDto)
                        .ToList();

            return items;
            
        }

        private FileVersionListDto MapFileVersionDto(FileVersion version)
        {
            return new FileVersionListDto
            {
                Id = version.Id,
                ArchiveId = version.FileArchiveId,
                FileName = version.FileArchive.FileName,
                VersionNumber = version.VersionNumber,
                ContentType = version.ContentType.Extension,
                CreatedAt = version.CreatedAt,
                CreatedBy = $"{version.Created.FirstName} {version.Created.LastName}"
            };
        }

        public async Task<bool> ValidateChecksum(string checksum, Guid archiveId)
        {
            var fileVersions = await _fileVersionRepository.GetAllAsync();
            var existingVersion = fileVersions.FirstOrDefault(fv => fv.Checksum == checksum && fv.FileArchiveId == archiveId);
            return existingVersion == null;
        }

        public async Task<ICollection<FileVersionListDto>> GetFileVersionsByArchiveId(Guid id)
        {
            var fileVersions = await _fileVersionRepository.GetAllAsync();

            fileVersions = fileVersions.Where(f => f.FileArchiveId == id);

            var items = fileVersions.Select(MapFileVersionDto).ToList();

            return items;
        }
    }
}