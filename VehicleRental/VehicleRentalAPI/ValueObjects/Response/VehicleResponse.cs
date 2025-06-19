using System.Text.Json.Serialization;

namespace VehicleRentalAPI.ValueObjects.Response;

public class VehicleResponse
{
    [JsonPropertyName("identificador")]
    public string Identifier { get; set; }
    [JsonPropertyName("ano")]
    public int Year { get; set; }
    [JsonPropertyName("modelo")]
    public string Model { get; set; }
    [JsonPropertyName("placa")]
    public string Plate { get; set; }
}
