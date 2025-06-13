namespace WarehouseFileArchiverAPI.Models.DTOs
{
    public class UserListDto
    {
        public string Username { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
        public bool IsDeleted { get; set; }
        
    }
}