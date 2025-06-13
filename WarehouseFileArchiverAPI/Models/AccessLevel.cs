
using System.ComponentModel.DataAnnotations;

namespace WarehouseFileArchiverAPI.Models
{
    public class AccessLevel
    {
        public Guid Id { get; set; }
        [Required]
        public string Access { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public ICollection<Role>? Roles { get; set; }
        public ICollection<Category>? Categories { get; set; }
    }
}