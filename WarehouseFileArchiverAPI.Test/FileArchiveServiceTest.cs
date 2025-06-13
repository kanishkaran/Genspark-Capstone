// using System;
// using System.Collections.Generic;
// using System.IO;
// using System.Linq;
// using System.Threading.Tasks;
// using Microsoft.AspNetCore.Http;
// using Moq;
// using NUnit.Framework;
// using WarehouseFileArchiverAPI.Interfaces;
// using WarehouseFileArchiverAPI.Models;
// using WarehouseFileArchiverAPI.Models.DTOs;
// using WarehouseFileArchiverAPI.Services;
// using WarehouseFileArchiverAPI.Exceptions;
// using Microsoft.AspNetCore.Hosting;

// namespace WarehouseFileArchiverAPI.Test
// {
//     [TestFixture]
//     public class FileArchiveServiceTest
//     {
//         private Mock<IWebHostEnvironment> _envMock;
//         private Mock<IRepository<Guid, FileArchive>> _fileArchiveRepoMock;
//         private Mock<IRepository<Guid, FileVersion>> _fileVersionRepoMock;
//         private Mock<IRepository<Guid, AccessLevel>> _accessLevelRepoMock;
//         private Mock<IRepository<Guid, Category>> _categoryRepoMock;
//         private Mock<IRepository<Guid, MediaType>> _mediaTypeRepoMock;
//         private Mock<IRepository<Guid, Role>> _roleRepoMock;
//         private Mock<IRepository<Guid, RoleCategoryAccess>> _roleCategoryAccessRepoMock;
//         private Mock<IRepository<Guid, Employee>> _employeeRepoMock;
//         private Mock<IFileVersionService> _fileVersionServiceMock;
//         private Mock<IMediaTypeService> _mediaTypeServiceMock;
//         private Mock<ICategoryService> _categoryServiceMock;
//         private Mock<IEmployeeService> _employeeServiceMock;
//         private Mock<IChecksumService> _checksumServiceMock;
//         private Mock<IAuditLogService> _auditLogServiceMock;
//         private FileArchiveService _service;

//         [SetUp]
//         public void Setup()
//         {
//             _envMock = new Mock<IWebHostEnvironment>();
//             _fileArchiveRepoMock = new Mock<IRepository<Guid, FileArchive>>();
//             _fileVersionRepoMock = new Mock<IRepository<Guid, FileVersion>>();
//             _accessLevelRepoMock = new Mock<IRepository<Guid, AccessLevel>>();
//             _categoryRepoMock = new Mock<IRepository<Guid, Category>>();
//             _mediaTypeRepoMock = new Mock<IRepository<Guid, MediaType>>();
//             _roleRepoMock = new Mock<IRepository<Guid, Role>>();
//             _roleCategoryAccessRepoMock = new Mock<IRepository<Guid, RoleCategoryAccess>>();
//             _employeeRepoMock = new Mock<IRepository<Guid, Employee>>();
//             _fileVersionServiceMock = new Mock<IFileVersionService>();
//             _mediaTypeServiceMock = new Mock<IMediaTypeService>();
//             _categoryServiceMock = new Mock<ICategoryService>();
//             _employeeServiceMock = new Mock<IEmployeeService>();
//             _checksumServiceMock = new Mock<IChecksumService>();
//             _auditLogServiceMock = new Mock<IAuditLogService>();

//             _envMock.Setup(e => e.ContentRootPath).Returns(Path.GetTempPath());

//             _service = new FileArchiveService(
//                 _envMock.Object,
//                 _fileArchiveRepoMock.Object,
//                 _fileVersionRepoMock.Object,
//                 _accessLevelRepoMock.Object,
//                 _categoryRepoMock.Object,
//                 _mediaTypeRepoMock.Object,
//                 _roleRepoMock.Object,
//                 _roleCategoryAccessRepoMock.Object,
//                 _employeeRepoMock.Object,
//                 _fileVersionServiceMock.Object,
//                 _mediaTypeServiceMock.Object,
//                 _categoryServiceMock.Object,
//                 _employeeServiceMock.Object,
//                 _checksumServiceMock.Object,
//                 _auditLogServiceMock.Object
//             );
//         }

//         [Test]
//         public async Task UploadFile_Success()
//         {
//             var fileMock = new Mock<IFormFile>();
//             var fileName = "test.txt";
//             var ms = new MemoryStream(new byte[] { 1, 2, 3 });
//             fileMock.Setup(f => f.FileName).Returns(fileName);
//             fileMock.Setup(f => f.Length).Returns(ms.Length);
//             fileMock.Setup(f => f.OpenReadStream()).Returns(ms);
//             fileMock.Setup(f => f.ContentType).Returns("text/plain");
//             fileMock.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), default)).Returns((Stream s, System.Threading.CancellationToken t) => ms.CopyToAsync(s));

//             var uploadDto = new FileUploadDto
//             {
//                 File = fileMock.Object,
//                 Category = "Docs"
//             };

//             var category = new Category { Id = Guid.NewGuid(), CategoryName = "Docs", AccessLevelId = Guid.NewGuid() };
//             var employee = new Employee { Id = Guid.NewGuid(), Email = "user@email.com" };
//             var role = new Role { Id = Guid.NewGuid(), RoleName = "User" };
//             var fileArchive = new FileArchive { Id = Guid.NewGuid(), FileName = fileName, UploadedById = employee.Id, CategoryId = category.Id, Status = false };
//             var mediaType = new MediaType { Id = Guid.NewGuid(), TypeName = "text/plain", Extension = ".txt" };

//             _categoryServiceMock.Setup(s => s.GetCategoryByName("Docs")).ReturnsAsync(category);
//             _employeeServiceMock.Setup(s => s.GetEmployeeByEmail("user@email.com")).ReturnsAsync(employee);
//             _roleRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Role> { role });
//             _roleCategoryAccessRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<RoleCategoryAccess> { new RoleCategoryAccess { RoleId = role.Id, CategoryId = category.Id, CanUpload = true } });
//             _fileArchiveRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<FileArchive>());
//             _fileArchiveRepoMock.Setup(r => r.AddAsync(It.IsAny<FileArchive>())).ReturnsAsync(fileArchive);
//             _checksumServiceMock.Setup(s => s.ComputeSHA256(It.IsAny<Stream>())).ReturnsAsync("checksum");
//             _fileVersionServiceMock.Setup(s => s.ValidateChecksum(It.IsAny<string>(), It.IsAny<Guid>())).ReturnsAsync(false); // false = unique
//             _fileVersionServiceMock.Setup(s => s.GetFileVersionByArchiveId(fileArchive.Id)).ReturnsAsync((FileVersion)null);
//             _mediaTypeServiceMock.Setup(s => s.GetMediaTypeByContentType("text/plain")).ReturnsAsync(mediaType);
//             _fileVersionRepoMock.Setup(r => r.AddAsync(It.IsAny<FileVersion>())).ReturnsAsync(new FileVersion());

//             var result = await _service.UploadFile(uploadDto, "user@email.com", "User");

//             Assert.That(result, Is.EqualTo("File Uploaded Successfully"));
//         }

//         [Test]
//         public async Task DownloadFile_Success()
//         {
//             var fileName = "test.txt";
//             var fileArchiveId = Guid.NewGuid();
//             var categoryId = Guid.NewGuid();
//             var contentTypeId = Guid.NewGuid();
//             var versionNumber = 1;
//             var category = new Category { Id = categoryId, CategoryName = "Docs", AccessLevelId = Guid.NewGuid() };
//             var accessLevel = new AccessLevel { Id = Guid.NewGuid(), Access = "User", IsActive = true };
//             var fileArchive = new FileArchive { Id = fileArchiveId, FileName = fileName, CategoryId = categoryId, Status = false };
//             var role = new Role { Id = Guid.NewGuid(), RoleName = "User" };
//             var roleCategoryAccess = new RoleCategoryAccess { RoleId = role.Id, CategoryId = categoryId, CanDownload = true };
//             var mediaType = new MediaType { Id = contentTypeId, TypeName = "text/plain", Extension = ".txt" };

//             // Prepare file path as expected by the service
//             var expectedDir = Path.Combine(_envMock.Object.ContentRootPath, "FileArchives");
//             Directory.CreateDirectory(expectedDir);
//             var expectedPath = Path.Combine(expectedDir, $"v{versionNumber}_{fileName}");
//             File.WriteAllBytes(expectedPath, new byte[] { 1, 2, 3 });

//             var fileVersion = new FileVersion
//             {
//                 FileArchiveId = fileArchiveId,
//                 VersionNumber = versionNumber,
//                 FilePath = $"v{versionNumber}_{fileName}",
//                 ContentTypeId = contentTypeId
//             };

//             _fileArchiveRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<FileArchive> { fileArchive });
//             _fileVersionServiceMock.Setup(s => s.GetFileVersionByVersionNumber(fileArchiveId, versionNumber)).ReturnsAsync(fileVersion);
//             _categoryRepoMock.Setup(r => r.GetByIdAsync(categoryId)).ReturnsAsync(category);
//             _accessLevelRepoMock.Setup(r => r.GetByIdAsync(category.AccessLevelId)).ReturnsAsync(accessLevel);
//             _roleRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Role> { role });
//             _roleCategoryAccessRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<RoleCategoryAccess> { roleCategoryAccess });
//             _mediaTypeRepoMock.Setup(r => r.GetByIdAsync(contentTypeId)).ReturnsAsync(mediaType);

//             var result = await _service.DownloadFile(fileName, versionNumber, "User");

//             Assert.That(result.FileName, Is.EqualTo($"v{versionNumber}_{fileName}"));
//             Assert.That(result.ContentType, Is.EqualTo("text/plain"));
//             Assert.That(result.FileContent, Is.Not.Null.And.Length.EqualTo(3));

//             File.Delete(expectedPath);
//         }
//     }
// }