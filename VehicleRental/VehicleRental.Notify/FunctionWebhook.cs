using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using System.Text.Json;
using VehicleRental.Notify.Entities;

namespace VehicleRental.Notify
{
    public class FunctionWebhook
    {
        public async Task FunctionHandler(SQSEvent evnt, ILambdaContext context)
        {
            foreach (SQSEvent.SQSMessage record in evnt.Records)
            {
                try
                {
                    context.Logger.LogInformation($"Mensagem recebida: {record.Body}");

                    VehicleRegisteredEvent? message = JsonSerializer.Deserialize<VehicleRegisteredEvent>(record.Body);

                    if (message is null)
                    {
                        context.Logger.LogWarning("Mensagem inválida ou incompleta.");
                        continue;
                    }

                    await WebhookNotifier.SendAsync(message, context);
                    context.Logger.LogInformation($"Webhook enviado");
                }
                catch (Exception ex)
                {
                    context.Logger.LogError($"Erro ao processar: {ex.Message}");
                    await WebhookNotifier.SendAsync("Erro ao processar mensagem", context, ex);
                    throw;
                }
            }
        }
    }
}
