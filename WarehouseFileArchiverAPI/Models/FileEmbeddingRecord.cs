using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.VectorData;

namespace WarehouseFileArchiverAPI.Models
{

    public class FileEmbeddingRecord
    {
        [VectorStoreKey]
        public string Id { get; set; } = string.Empty;

        [VectorStoreData(IsFullTextIndexed = true)]
        public string TextContent { get; set; } = string.Empty;

        [VectorStoreVector(Dimensions: 1024, DistanceFunction = DistanceFunction.CosineSimilarity)]
        public ReadOnlyMemory<float>? Embedding { get; set; }

        [VectorStoreData(IsIndexed = true)]
        public string FileName { get; set; } = string.Empty;

        [VectorStoreData(IsIndexed = true)]
        public string ContentType { get; set; } = string.Empty;
    }

}