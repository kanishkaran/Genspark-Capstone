using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WarehouseFileArchiverAPI.Interfaces
{
    public interface IFileSummaryService
    {
        Task<string> SummarizeByFileNameAsync(string fileName, string role);
        Task<object> SemanticSearchForFileName(string query, string role);
    }
}