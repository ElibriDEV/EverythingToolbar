using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using EverythingToolbar.Properties;
using NLog;

namespace EverythingToolbar.Helpers
{
    internal class AIClient
    {
        private static readonly ILogger Logger = ToolbarLogger.GetLogger<AIClient>();
        private static readonly HttpClient HttpClient = new HttpClient();
        private const string ApiUrl = "https://n8n-dev.finam.ru/webhook/7c1e771d-e101-4d36-a83a-e3023ecc767c";

        private class AIRequest
        {
            public string user_request { get; set; }
        }

        private class AIResponse
        {
            public string output { get; set; }
        }

        public static async Task<string> GetResponse(string query)
        {
            try
            {
                var request = new AIRequest { user_request = query };
                var jsonRequest = JsonSerializer.Serialize(request);
                var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                var response = await HttpClient.PostAsync(ApiUrl, content);
                response.EnsureSuccessStatusCode();

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var aiResponse = JsonSerializer.Deserialize<AIResponse>(jsonResponse);

                return aiResponse?.output ?? Resources.AIApiError;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Failed to get AI response.");
                return Resources.AIApiError;
            }
        }
    }
}
