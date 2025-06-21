using System.Threading.Tasks;
using WarehouseFileArchiverAPI.Exceptions;
using WarehouseFileArchiverAPI.Interfaces;
using WarehouseFileArchiverAPI.Mappers;
using WarehouseFileArchiverAPI.Models;
using WarehouseFileArchiverAPI.Models.DTOs;

namespace WarehouseFileArchiverAPI.Services
{
    public class FileArchiveService : IFileArchiveService
    {
        private readonly IMediaTypeService _mediaTypeService;
        private readonly ICategoryService _categoryService;
        private readonly IChecksumService _checksumService;
        private readonly IEmployeeService _employeeService;
        private readonly IAuditLogService _auditLogService;
        private readonly FileArchiveMapper _mapper;
        private readonly IWebHostEnvironment _env;
        private readonly IRepository<Guid, FileArchive> _fileArchiveRepository;
        private readonly IRepository<Guid, FileVersion> _fileVersionRepository;
        private readonly IRepository<Guid, AccessLevel> _accessLevelRepository;
        private readonly IRepository<Guid, Employee> _employeeRepository;
        private readonly IRepository<Guid, Category> _categoryRepository;
        private readonly IRepository<Guid, MediaType> _mediaTypeRepository;
        private readonly IRepository<Guid, Role> _roleRepository;
        private readonly IRepository<Guid, RoleCategoryAccess> _roleCategoryAccessRepository;
        private readonly IFileVersionService _fileVersionService;

        public FileArchiveService(IWebHostEnvironment environment,
                                  IRepository<Guid, FileArchive> fileArchiveRepository,
                                  IRepository<Guid, FileVersion> fileVersionRepository,
                                  IRepository<Guid, AccessLevel> accessLevelRepository,
                                  IRepository<Guid, Category> categoryRepository,
                                  IRepository<Guid, MediaType> mediaTypeRepository,
                                  IRepository<Guid, Role> roleRepository,
                                  IRepository<Guid, RoleCategoryAccess> roleCategoryAccessRepository,
                                  IRepository<Guid, Employee> employeeRepository,
                                  IFileVersionService fileVersionService,
                                  IMediaTypeService mediaTypeService,
                                  ICategoryService categoryService,
                                  IEmployeeService employeeService,
                                  IChecksumService checksumService,
                                  IAuditLogService auditLogService)
        {
            _env = environment;
            _fileArchiveRepository = fileArchiveRepository;
            _fileVersionRepository = fileVersionRepository;
            _accessLevelRepository = accessLevelRepository;
            _employeeRepository = employeeRepository;
            _categoryRepository = categoryRepository;
            _mediaTypeRepository = mediaTypeRepository;
            _roleRepository = roleRepository;
            _roleCategoryAccessRepository = roleCategoryAccessRepository;

            _fileVersionService = fileVersionService;
            _mediaTypeService = mediaTypeService;
            _categoryService = categoryService;
            _checksumService = checksumService;

            _employeeService = employeeService;

            _auditLogService = auditLogService;

            _mapper = new();

        }

        public async Task<PaginationDto<FileArchiveResponseDto>> SearchFileArchives(SearchQueryDto searchDto)
        {
            var fileArchives = await _fileArchiveRepository.GetAllAsync() ?? throw new CollectionEmptyException("No file archives in the Database");


            if (!searchDto.IncludeInactive)
                fileArchives = fileArchives.Where(fa => !fa.Status);

            if (!string.IsNullOrWhiteSpace(searchDto.Search))
            {
                var search = searchDto.Search.ToLower();
                fileArchives = fileArchives.Where(fa => fa.FileName.ToLower().Contains(search));
            }

            fileArchives = searchDto.SortBy?.ToLower() switch
            {
                "filename" => searchDto.Desc ? fileArchives.OrderByDescending(fa => fa.FileName) : fileArchives.OrderBy(fa => fa.FileName),
                _ => searchDto.Desc ? fileArchives.OrderByDescending(fa => fa.Id) : fileArchives.OrderBy(fa => fa.Id)
            };

            var totalRecords = fileArchives.Count();
            if (totalRecords == 0)
                throw new CollectionEmptyException("No FileArchive Matched");


            var items = fileArchives
                .Skip((searchDto.Page - 1) * searchDto.PageSize)
                .Take(searchDto.PageSize)
                .Select(fa => new FileArchiveResponseDto
                {
                    Id = fa.Id,
                    FileName = fa.FileName,
                    UploadedById = fa.UploadedById,
                    UploadedByName = fa.Employee?.FirstName ?? "",
                    CategoryId = fa.CategoryId,
                    CategoryName = fa.Category?.CategoryName ?? "",
                    Status = fa.Status
                })
                .ToList();

            return new PaginationDto<FileArchiveResponseDto>
            {
                Data = items,
                TotalRecords = totalRecords,
                Page = searchDto.Page,
                PageSize = searchDto.PageSize,
                TotalPages = (int)Math.Ceiling(totalRecords / (double)searchDto.PageSize)
            };
        }

        public async Task<FileArchiveResponseDto> GetById(Guid id)
        {
            var fileArchive = await _fileArchiveRepository.GetByIdAsync(id);
            if (fileArchive == null || fileArchive.Status)
                throw new FileArchiveNotFoundException($"FileArchive with id: {id} was not found or deleted.");

            var category = await _categoryRepository.GetByIdAsync(fileArchive.CategoryId);
            var employee = await _employeeRepository.GetByIdAsync(fileArchive.UploadedById);
            return new FileArchiveResponseDto
            {
                Id = fileArchive.Id,
                FileName = fileArchive.FileName,
                UploadedById = fileArchive.UploadedById,
                UploadedByName = employee.FirstName + " " + employee.LastName ?? "",
                CategoryId = fileArchive.CategoryId,
                CategoryName = category.CategoryName ?? "",
                Status = fileArchive.Status
            };
        }

        public async Task<FileDownloadDto> DownloadFile(string fileName, int versionNumber, string role)
        {
            try
            {
                var fileArchive = await GetByFileName(fileName);
                if (fileArchive == null || fileArchive.Status)
                    throw new FileArchiveNotFoundException($"No Such File as {fileName}");

                FileVersion fileVersion;
                if (versionNumber > 0)
                    fileVersion = await _fileVersionService.GetFileVersionByVersionNumber(fileArchive.Id, versionNumber);
                else
                    fileVersion = await _fileVersionService.GetFileVersionByArchiveId(fileArchive.Id);

                if (fileVersion == null)
                    throw new VersionMismatchException($"There is no file {fileName} of version number - {versionNumber}");


                var category = await _categoryRepository.GetByIdAsync(fileArchive.CategoryId);
                var accessLevel = await _accessLevelRepository.GetByIdAsync(category.AccessLevelId);
                if (accessLevel.Access == "Admin" && role != "Admin")
                    throw new UnauthorizedAccessException($"Cannot Download file from the Category {category.CategoryName}");


                var roles = await _roleRepository.GetAllAsync();
                var userRole = roles.FirstOrDefault(r => r.RoleName.Equals(role, StringComparison.OrdinalIgnoreCase));
                if (userRole == null)
                    throw new Exception($"Role '{role}' does not exist.");


                if (userRole.RoleName != "Admin")
                {
                    var roleCategoryAccesses = await _roleCategoryAccessRepository.GetAllAsync();
                    var categoryAccess = roleCategoryAccesses.FirstOrDefault(r =>
                        r.RoleId == userRole.Id && r.CategoryId == category.Id);

                    if (categoryAccess == null || !categoryAccess.CanDownload)
                        throw new AccessViolationException($"You do not have download permission for category '{category.CategoryName}'.");

                }

                var filePath = Path.Combine(_env.ContentRootPath, $"Uploads/{category.CategoryName}", fileVersion.FilePath);

                if (!File.Exists(filePath))
                    throw new FileNotFoundException("File not found on disk.", filePath);

                var bytes = await File.ReadAllBytesAsync(filePath);

                var mediaType = await _mediaTypeRepository.GetByIdAsync(fileVersion.ContentTypeId);

                return new FileDownloadDto
                {
                    ContentType = mediaType.TypeName ?? "application/octet-stream",
                    FileContent = bytes,
                    FileName = $"v{versionNumber}_{fileArchive.FileName}"
                };
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<string> UploadFile(FileUploadDto files, string userName, string role)
        {
            var file = files.File;
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is invalid");

            var category = await _categoryService.GetCategoryByName(files.Category);
            var user = await _employeeService.GetEmployeeByEmail(userName);
            var userRole = await GetRoleByName(role);

            await EnsureCategoryPermission(category, role);

            if (role != "Admin")
            {
                await EnsureUploadPermission(userRole.Id, category);
            }

            var fileArchive = await GetOrCreateFileArchive(files, user.Id, category.Id);

            var checksum = await _checksumService.ComputeSHA256(file.OpenReadStream());
            await EnsureUniqueFileVersion(checksum, fileArchive.Id);

            var versionNumber = await GetNextVersionNumber(fileArchive.Id);

            var filePath = await SaveFileToDisk(file, category.CategoryName);

            var mediaType = await _mediaTypeService.GetMediaTypeByContentType(file.ContentType);

            var version = new FileVersion
            {
                FileArchiveId = fileArchive.Id,
                VersionNumber = versionNumber,
                FilePath = Path.GetFileName(filePath),
                ContentTypeId = mediaType.Id,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = user.Id,
                Checksum = checksum
            };

            await _fileVersionRepository.AddAsync(version);

            return "File Uploaded Successfully";
        }

        private async Task EnsureCategoryPermission(Category category, string role)
        {
            var access = await _accessLevelRepository.GetByIdAsync(category.AccessLevelId);
            if (access.Access.ToLower() == "read-only" || access.Access.ToLower() == "admin" && role.ToLower() != "admin")
                throw new UnauthorizedAccessException($"Cannot Upload in {category.CategoryName}, it is a {access.Access} category");
            return;
        }

        private async Task<Role> GetRoleByName(string roleName)
        {
            var roles = await _roleRepository.GetAllAsync();
            return roles.FirstOrDefault(r => r.RoleName.Equals(roleName, StringComparison.OrdinalIgnoreCase))
                ?? throw new RoleNotFoundException($"The role: {roleName} was not found");
        }

        private async Task EnsureUploadPermission(Guid roleId, Category category)
        {
            var roleCategoryAccesses = await _roleCategoryAccessRepository.GetAllAsync();
            var categoryAccess = roleCategoryAccesses.FirstOrDefault(r =>
                r.RoleId == roleId && r.CategoryId == category.Id);

            if (categoryAccess == null || !categoryAccess.CanUpload)
                throw new AccessViolationException($"You do not have upload permission for category '{category.CategoryName}'.");
        }

        private async Task<FileArchive> GetOrCreateFileArchive(FileUploadDto files, Guid userId, Guid categoryId)
        {
            var fileArchive = await GetByFileName(files.File.FileName);

            if (fileArchive == null)
            {
                fileArchive = _mapper.MapFileArchiveDtoToFileArchive(files);
                fileArchive.UploadedById = userId;
                fileArchive.CategoryId = categoryId;
                fileArchive.Status = false;
                await _fileArchiveRepository.AddAsync(fileArchive);
            }

            return fileArchive;
        }

        private async Task EnsureUniqueFileVersion(string checksum, Guid fileArchiveId)
        {
            var isValid = await _fileVersionService.ValidateChecksum(checksum, fileArchiveId);
            if (!isValid)
                throw new VersionMismatchException("This version of file already exists");
        }

        private async Task<int> GetNextVersionNumber(Guid fileArchiveId)
        {
            var fileVersion = await _fileVersionService.GetFileVersionByArchiveId(fileArchiveId);
            return fileVersion == null ? 1 : fileVersion.VersionNumber + 1;
        }

        private async Task<string> SaveFileToDisk(IFormFile file, string categoryName)
        {
            var folder = Path.Combine(_env.ContentRootPath, $"Uploads/{categoryName}");
            Directory.CreateDirectory(folder);

            var mediaType = await _mediaTypeService.GetMediaTypeByContentType(file.ContentType);
            var filePath = Path.Combine(folder, $"{Guid.NewGuid()}{mediaType.Extension}");

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return filePath;
        }

        private async Task<FileArchive?> GetByFileName(string fileName)
        {
            var files = await _fileArchiveRepository.GetAllAsync();

            return files.FirstOrDefault(f => f.FileName == fileName && !f.Status);

        }

        public async Task<bool> DeleteFileArchive(Guid id, string currUser)
        {
            var fileArchive = await _fileArchiveRepository.GetByIdAsync(id);
            if (fileArchive == null || fileArchive.Status)
                throw new FileArchiveNotFoundException($"FileArchive with id: {id} was not found or already deleted.");

            fileArchive.Status = true;
            await _fileArchiveRepository.UpdateAsync(id, fileArchive);
            await _auditLogService.LogAsync(
                "File Archive",
                fileArchive.Id,
                "Delete",
                currUser ?? "system",
                $"File Archive with id {id} marked as deleted."
            );

            return true;
        }


    }
}