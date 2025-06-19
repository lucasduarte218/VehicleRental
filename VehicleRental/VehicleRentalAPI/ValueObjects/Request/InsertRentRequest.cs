using System.Text.Json.Serialization;

namespace VehicleRentalAPI.ValueObjects.Request;

public class InsertRentRequest
{
    [JsonPropertyName("identificador")]
    public string? Identifier { get; set; }

    [JsonPropertyName("entregador_id")]
    public string CourierIdentifier { get; set; }

    [JsonPropertyName("moto_id")]
    public string MotorcycleIdentifier { get; set; }

    [JsonPropertyName("data_inicio")]
    public DateTime StartDate { get; set; }

    [JsonPropertyName("data_termino")]
    public DateTime EndDate { get; set; }

    [JsonPropertyName("data_previsao_termino")]
    public DateTime EstimatedEndDate { get; set; }

    [JsonPropertyName("plano")]
    public required int Plan { get; set; }
}
