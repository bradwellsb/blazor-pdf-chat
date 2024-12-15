using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.VectorData;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Parsing;
using System.Text.RegularExpressions;

namespace BlazorPdfChat.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VectorsController : ControllerBase
    {
        private readonly PredictionGuardEmbeddingsClient _predictionGuardEmbeddingsClient;
        private readonly IVectorStoreRecordCollection<string, TextSnippet> _collection;

        public VectorsController(PredictionGuardEmbeddingsClient predictionGuardEmbeddingClient, IVectorStore vectorStore)
        {
            _predictionGuardEmbeddingsClient = predictionGuardEmbeddingClient;
            _collection = vectorStore.GetCollection<string, TextSnippet>("mfp");
            InitializeAsync().GetAwaiter().GetResult();
        }
        private async Task InitializeAsync()
        {
            await _collection.CreateCollectionIfNotExistsAsync();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Vectorize(List<IFormFile> FilesToUpload)
        {
            if (FilesToUpload.Count > 0)
            {
                foreach (var file in FilesToUpload)
                {
                    var contentItems = ExtractTextFromPdf(file);

                    foreach (var contentItem in contentItems)
                    {
                        var textSnippet = new TextSnippet
                        {
                            Key = Guid.NewGuid().ToString(),
                            Text = contentItem.Text,
                            TextEmbedding = await _predictionGuardEmbeddingsClient.GenerateEmbeddingsAsync(contentItem.Text),
                            PageNumber = $"Page {contentItem.PageNumber}",
                            FileName = file.FileName
                        };
                        await _collection.UpsertAsync(textSnippet);
                    }
                }
            }
            return Ok();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Search([FromBody] string userMessage)
        {
            ReadOnlyMemory<float> searchVector = await _predictionGuardEmbeddingsClient.GenerateEmbeddingsAsync(userMessage);
            var searchResult = await _collection.VectorizedSearchAsync(searchVector, new() { Top = 3, IncludeTotalCount = true });
            return Ok(searchResult);
        }

        private static IEnumerable<RawContent> ExtractTextFromPdf(IFormFile pdfFile)
        {
            using (var stream = pdfFile.OpenReadStream())
            using (PdfLoadedDocument document = new PdfLoadedDocument(stream))
            {
                for (int i = 0; i < document.Pages.Count; i++)
                {
                    PdfLoadedPage page = document.Pages[i] as PdfLoadedPage;

                    // Extract text from the page
                    string text = page.ExtractText();
                    text = RemoveExtraWhiteSpace(text);
                    var chunks = ChunkText(text, 1000); // Chunk the text into smaller sections of 1000 characters

                    foreach (var chunk in chunks)
                    {
                        yield return new RawContent { Text = chunk, PageNumber = i + 1 };
                    }
                }
            }
        }

        private static string RemoveExtraWhiteSpace(string text)
        {
            // Replace multiple whitespace characters with a single space
            if(text == null)
                return string.Empty;
            return Regex.Replace(text, @"\s+", " ").Trim();
        }

        private static IEnumerable<string> ChunkText(string text, int chunkSize)
        {
            for (int i = 0; i < text.Length; i += chunkSize)
            {
                yield return text.Substring(i, Math.Min(chunkSize, text.Length - i));
            }
        }

        public sealed class RawContent
        {
            public string? Text { get; init; }
            public int PageNumber { get; init; }
        }
    }
}
