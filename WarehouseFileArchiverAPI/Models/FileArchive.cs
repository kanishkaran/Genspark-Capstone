
using System.ComponentModel.DataAnnotations;

namespace WarehouseFileArchiverAPI.Models
{
    public class FileArchive
    {
        public Guid Id { get; set; }
        [Required]
        public string FileName { get; set; } = string.Empty;
        public Guid UploadedById { get; set; }
        public Guid CategoryId { get; set; }
        public bool Status { get; set; }
        public Category? Category { get; set; }

        public bool CanSummarise { get; set; }

        public ICollection<FileVersion>? FileVersions { get; set; }
        public Employee? Employee { get; set; }
    }
}