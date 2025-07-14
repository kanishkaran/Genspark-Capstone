using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WarehouseFileArchiverAPI.Models;
using WarehouseFileArchiverAPI.Models.DTOs;

namespace WarehouseFileArchiverAPI.Interfaces
{
    public interface IFileArchiveService
    {
        Task<PaginationDto<FileArchiveResponseDto>> SearchFileArchives(SearchQueryDto searchDto);
        Task<FileArchiveResponseDto> GetById(Guid id);
        Task<string> UploadFile(FileUploadDto files, string userName, string role);
        Task<FileDownloadDto> DownloadFile(string fileName, int versionNumber, string role);
        Task<bool> DeleteFileArchive(Guid id, string currUser);

        Task<FileArchive?> GetByFileName(string fileName);

        Task EnsureDownloadPermission(Guid roleId, Category category);

        Task<Role> GetRoleByName(string roleName);
    }
}