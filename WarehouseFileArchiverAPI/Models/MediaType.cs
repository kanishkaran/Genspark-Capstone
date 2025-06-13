
using System.ComponentModel.DataAnnotations;

namespace WarehouseFileArchiverAPI.Models
{
    public class MediaType
    {
        public Guid Id { get; set; }
        [Required]
        public string TypeName { get; set; } = string.Empty;
        [Required]
        public string Extension { get; set; } = string.Empty;
        public bool IsDeleted { get; set; } = false;
        public ICollection<FileVersion>? FileVersions { get; set; }
    }
}