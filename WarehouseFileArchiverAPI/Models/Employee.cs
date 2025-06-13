

using System.ComponentModel.DataAnnotations;

namespace WarehouseFileArchiverAPI.Models
{
    public class Employee
    {
        public Guid Id { get; set; }
        [Required]
        public string FirstName { get; set; } = string.Empty;
        [Required]
        public string LastName { get; set; } = string.Empty;
        [Required]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;
        [Required]
        
        public string ContactNumber { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public User? User { get; set; }

        public ICollection<FileArchive>? FileArchives { get; set; }
    }
}