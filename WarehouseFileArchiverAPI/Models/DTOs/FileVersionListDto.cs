using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WarehouseFileArchiverAPI.Models.DTOs
{
    public class FileVersionListDto
    {
        public Guid Id { get; set; }
        public Guid ArchiveId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public int VersionNumber { get; set; }
        public string ContentType { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
    }
}