using Microsoft.Extensions.VectorData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorPdfChat
{
    public class TextSnippet
    {
        [VectorStoreRecordKey]
        public required string Key { get; set; }

        [VectorStoreRecordData]
        public string? Text { get; set; }

        [VectorStoreRecordData]
        public string? PageNumber { get; set; }

        [VectorStoreRecordData]
        public string? FileName { get; set; }

        [VectorStoreRecordVector]
        public ReadOnlyMemory<float> TextEmbedding { get; set; }
    }
}
