using Microsoft.Extensions.VectorData;

namespace PredictionGuardPdfChat.Services
{
    public class VectorsService
    {
        private readonly HttpClient _httpClient;

        public VectorsService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<VectorSearchResults<TextSnippet>> SearchAsync(string userMessage)
        {
            var response = await _httpClient.PostAsJsonAsync("api/vectors/Search", userMessage);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<VectorSearchResults<TextSnippet>>();
        }
    }
}
