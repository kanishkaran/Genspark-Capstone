using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WarehouseFileArchiverAPI.Models.DTOs
{
    public class FileEmbeddingResultDto
    {
        public bool CanSummarise { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}