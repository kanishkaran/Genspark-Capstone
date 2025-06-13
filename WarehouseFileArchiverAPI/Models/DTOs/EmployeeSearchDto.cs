namespace WarehouseFileArchiverAPI.Models.DTOs
{
    public class EmployeeSearchDto
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? ContactNumber { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SortBy { get; set; }
        public bool Desc { get; set; } = false;
        public bool IncludeInactive { get; set; } = false;
    }
}