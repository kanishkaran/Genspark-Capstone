
using System.ComponentModel.DataAnnotations;

namespace WarehouseFileArchiverAPI.Models
{
    public class FileVersion
    {
        public Guid Id { get; set; }
        public Guid FileArchiveId { get; set; }
        public int VersionNumber { get; set; }
        [Required]
        public string Checksum { get; set; } = string.Empty;
        [Required]
        public string FilePath { get; set; } = string.Empty;
        public Guid ContentTypeId { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid CreatedBy { get; set; }

        public FileArchive? FileArchive { get; set; }
        public MediaType? ContentType { get; set; }
        public User? Created { get; set; }
    }
}