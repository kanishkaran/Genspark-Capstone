using Microsoft.SemanticKernel.Connectors.PgVector;
using Microsoft.SemanticKernel.Embeddings;
using OllamaSharp;
using WarehouseFileArchiverAPI.Exceptions;
using WarehouseFileArchiverAPI.Interfaces;
using WarehouseFileArchiverAPI.Models;


namespace WarehouseFileArchiverAPI.Services
{
    public class FileSummaryService : IFileSummaryService
    {
        private readonly PostgresCollection<string, FileEmbeddingRecord> _collection;
        private readonly ISummarizationService _summarization;
        private readonly OllamaApiClient _ollamaClient;
        private readonly IFileArchiveService _fileArchiveService;
        private readonly IRepository<Guid, Category> _categoryRepo;
        private readonly IRepository<Guid, RoleCategoryAccess> _roleAccessRepo;
        private readonly IRepository<Guid, AccessLevel> _accessLevelRepository;

        public FileSummaryService(ISummarizationService summarization,
         IFileArchiveService fileArchiveService,
                                     IRepository<Guid, Category> categoryRepository,
                                     IRepository<Guid, RoleCategoryAccess> roleCategoryAccessRepo,
                                     IRepository<Guid, AccessLevel> accessLevelRepository
        )
        {
            _summarization = summarization;

            _ollamaClient = new OllamaApiClient(new Uri("http://localhost:11434"), "mxbai-embed-large");
            _collection = new PostgresCollection<string, FileEmbeddingRecord>(
                "User ID=postgres;Password=[]kanish;Host=localhost;Port=5432;Database=WarehouseArchiveDB;",
                "FileEmbeddings"
            );

            _fileArchiveService = fileArchiveService;
            _categoryRepo = categoryRepository;
            _roleAccessRepo = roleCategoryAccessRepo;
            _accessLevelRepository = accessLevelRepository;
        }

        public async Task<string> SummarizeByFileNameAsync(string fileName, string role)
        {
            // if (!fileName.Contains(".pdf") || !fileName.Contains(".doc") || !fileName.Contains(".docx"))
            //     throw new Exception("Only Pdfs and Word documents can be summarised");
            var fileArchive = await _fileArchiveService.GetByFileName(fileName);
            if (fileArchive == null || fileArchive.Status)
                throw new FileArchiveNotFoundException($"No Such File as {fileName}");

            var category = await _categoryRepo.GetByIdAsync(fileArchive.CategoryId);
            var accessLevel = await _accessLevelRepository.GetByIdAsync(category.AccessLevelId);
            if (accessLevel.Access == "Admin" && role != "Admin")
                throw new UnauthorizedAccessException($"Cannot Download file from the Category {category.CategoryName} It is a Admin only Category");


            var userRole = await _fileArchiveService.GetRoleByName(role);
            if (userRole == null)
                throw new Exception($"Role '{role}' does not exist.");


            if (userRole.RoleName != "Admin")
            {
                await EnsureSummarisePermission(userRole.Id, category);
            }

            await _collection.EnsureCollectionExistsAsync();

            var matchingRecords = _collection.GetAsync(r => r.FileName == fileName, top: 1);
            await foreach (var record in matchingRecords)
            {
                if (!string.IsNullOrWhiteSpace(record.TextContent))
                {
                    return await _summarization.SummarizeAsync(record.TextContent);
                }
            }

            throw new Exception("No matching file found in vector store.");
        }

        private async Task EnsureSummarisePermission(Guid id, Category category)
        {
            var roleCategoryAccesses = await _roleAccessRepo.GetAllAsync();
            var categoryAccess = roleCategoryAccesses.FirstOrDefault(r =>
                r.RoleId == id && r.CategoryId == category.Id);

            if (categoryAccess == null || !categoryAccess.CanDownload)
                throw new AccessViolationException($"You do not have permission for category '{category.CategoryName}' to summarize.");
        }


        public async Task<object> SemanticSearchForFileName(string query, string role)
        {
            if (role != "Admin")
                throw new UnauthorizedAccessException("Admin Access Only");

            if (!string.IsNullOrWhiteSpace(query))
            {
                var embedder = _ollamaClient.AsTextEmbeddingGenerationService();
                var searchEmbedding = await embedder.GenerateEmbeddingAsync(query);

                var result = _collection.SearchAsync(searchEmbedding, top: 1);

                await foreach (var item in result)
                {
                    var summary = await _summarization.SummarizeAsync(item.Record.TextContent);
                    return new { fileName = item.Record.FileName, fileSummary = summary, confidenceScore = item.Score };
                }
            }
            return string.Empty;
        }
    }
}
