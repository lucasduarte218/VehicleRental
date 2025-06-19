using Amazon.Lambda.Core;
using System.Text;
using System.Text.Json;

namespace VehicleRental.Notify
{
    public static class WebhookNotifier
    {
        private static readonly HttpClient _httpClient = new();

        private const string WebhookUrl = "https://webhook.site/71fc835f-f5f2-4ab9-8663-6ccbeaedea62"; 

        public static async Task SendAsync(object message, ILambdaContext context, Exception? exception = null)
        {
            var payload = new
            {
                Timestamp = DateTime.UtcNow,
                LambdaFunctionName = context.FunctionName,
                RequestId = context.AwsRequestId,
                LogGroup = context.LogGroupName,
                LogStream = context.LogStreamName,
                Message = message,
                Error = exception?.ToString()
            };

            string json = JsonSerializer.Serialize(payload);
            StringContent content = new(json, Encoding.UTF8, "application/json");

            using var response = await _httpClient.PostAsync(WebhookUrl, content);
            response.EnsureSuccessStatusCode();
        }
    }
}
