using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WarehouseFileArchiverAPI.Interfaces;
using WarehouseFileArchiverAPI.Models;
using WarehouseFileArchiverAPI.Models.DTOs;

namespace WarehouseFileArchiverAPI.Mappers
{
    public class FileArchiveMapper
    {
        public FileArchive MapFileArchiveDtoToFileArchive(FileUploadDto files)
        {
            FileArchive fileArchive = new();
            fileArchive.FileName = files.File.FileName;
            return fileArchive;
        }
    }
}