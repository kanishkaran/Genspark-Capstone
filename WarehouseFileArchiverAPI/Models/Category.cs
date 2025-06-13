
using System.ComponentModel.DataAnnotations;

namespace WarehouseFileArchiverAPI.Models
{
    public class Category
    {
        public Guid Id { get; set; }
        [Required]
        public string CategoryName { get; set; } = string.Empty;
        public Guid AccessLevelId { get; set; }
        public bool IsDeleted { get; set; } = false;
        public ICollection<FileArchive>? FileArchives { get; set; }

        public AccessLevel? AccessLevel { get; set; }
        public List<RoleCategoryAccess>? RoleCategoryAccesses { get; set; }
    }
}