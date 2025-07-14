using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WarehouseFileArchiverAPI.Interfaces
{
    public interface IFileTextExtractorService
    {
        Task<string> ExtractTextAsync(IFormFile file);
    }

}