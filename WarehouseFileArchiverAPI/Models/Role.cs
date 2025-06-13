
using System.ComponentModel.DataAnnotations;

namespace WarehouseFileArchiverAPI.Models
{
    public class Role
    {
        public Guid Id { get; set; }
        [Required]
        public string RoleName { get; set; } = string.Empty;
        public Guid AccessLevelId { get; set; }
        public bool IsDeleted { get; set; } = false;
        public ICollection<User>? Users { get; set; }
        public AccessLevel? AccessLevel { get; set; }
        public List<RoleCategoryAccess>? RoleCategoryAccesses { get; set; }
    }
}