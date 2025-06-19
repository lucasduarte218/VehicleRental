using System.Text.Json.Serialization;
using VehicleRental.Domain.Enums;

namespace VehicleRentalAPI.ValueObjects.Response;

public class RentDetailsResponse
{
    [JsonPropertyName("identificador")]
    public required string Identifier { get; set; }

    [JsonPropertyName("valor_diaria")]
    public decimal DailyRate { get; set; }

    [JsonPropertyName("entregador_id")]
    public required string CourierIdentifier { get; set; }

    [JsonPropertyName("moto_id")]
    public required string MotorcycleIdentifier { get; set; }

    [JsonPropertyName("data_inicio")]
    public DateTime StartDate { get; set; }

    [JsonPropertyName("data_termino")]
    public DateTime? EndDate { get; set; }

    [JsonPropertyName("data_previsao_termino")]
    public DateTime EstimatedEndDate { get; set; }

    [JsonPropertyName("data_devolucao")]
    public DateTime? ReturnDate { get; set; }

    [JsonPropertyName("status")]
    public RentStatus Status { get; set; }
}
