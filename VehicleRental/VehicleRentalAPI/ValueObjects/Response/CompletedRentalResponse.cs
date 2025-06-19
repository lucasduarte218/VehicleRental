using System.Text.Json.Serialization;
using VehicleRental.Domain.Enums;

namespace VehicleRentalAPI.ValueObjects.Response;

public class CompletedRentalResponse
{
    [JsonPropertyName("identificador")]
    public string Identifier { get; set; }

    [JsonPropertyName("identificador_moto")]
    public string MotorcycleIdentifier { get; set; }

    [JsonPropertyName("identificador_entregador")]
    public string CourierIdentifier { get; set; }

    [JsonPropertyName("data_inicio")]
    public DateTime StartDate { get; set; }

    [JsonPropertyName("data_fim")]
    public DateTime EndDate { get; set; }

    [JsonPropertyName("custo_total")]
    public decimal TotalCost { get; set; }

    [JsonPropertyName("status")]
    public RentStatus Status { get; set; }
}
