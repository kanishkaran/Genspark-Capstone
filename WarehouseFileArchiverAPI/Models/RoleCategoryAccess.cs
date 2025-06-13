using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WarehouseFileArchiverAPI.Models
{
    public class RoleCategoryAccess
    {
        public Guid Id { get; set; }
        public Guid RoleId { get; set; }
        public Guid CategoryId { get; set; }

        [Required]
        public bool CanUpload { get; set; }
        [Required]
        public bool CanDownload { get; set; }

        public bool IsDeleted { get; set; } = false;
        public Role? Role { get; set; }
        public Category? Category { get; set; }
    }
}