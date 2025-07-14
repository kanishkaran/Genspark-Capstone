using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WarehouseFileArchiverAPI.Interfaces
{
    public interface ISummarizationService
    {
        Task<string> SummarizeAsync(string text);
    }
}