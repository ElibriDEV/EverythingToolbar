using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using EverythingToolbar.Properties;
using Microsoft.Win32;
using NLog;

namespace EverythingToolbar.Helpers
{
    internal class AIClient
    {
        private static readonly ILogger Logger = ToolbarLogger.GetLogger<AIClient>();
        private static readonly HttpClient HttpClient = new HttpClient();
        private const string ApiUrl = "https://n8n.whotrades.com/webhook/7c1e771d-e101-4d36-a83a-e3023ecc767c";
        private static CancellationTokenSource _cancellationTokenSource;

        private class AIRequest
        {
            public string user_request { get; set; }
            public Metadata metadata { get; set; }
        }

        private class Metadata
        {
            public string device_name { get; set; }
            public string device_code { get; set; }
        }


        private class AIResponse
        {
            public string output { get; set; }
        }

        public static async Task<string> GetResponse(string query)
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = _cancellationTokenSource.Token;

            try
            {
                string path = Path.Combine(Registry.LocalMachine.Name, @"SOFTWARE\Microsoft\SQMClient");
                Guid machineId = new Guid((string)Registry.GetValue(path, "MachineId", null));
                var request = new AIRequest
                {
                    user_request = query,
                    metadata = new Metadata
                    {
                        device_name = Environment.MachineName,
                        device_code = machineId.ToString()
                    }
                };
                var jsonRequest = JsonSerializer.Serialize(request);
                var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                var response = await HttpClient.PostAsync(ApiUrl, content, cancellationToken);
                response.EnsureSuccessStatusCode();

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var aiResponse = JsonSerializer.Deserialize<AIResponse>(jsonResponse);

                return aiResponse?.output ?? Resources.AIApiError;
            }
            catch (OperationCanceledException)
            {
                Logger.Info("AI request was cancelled.");
                return null;
            }
            catch (Exception ex)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    Logger.Info("AI request was cancelled.");
                    return null;
                }
                Logger.Error(ex, "Failed to get AI response.");
                return Resources.AIApiError;
            }
        }
    }
}
