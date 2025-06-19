using System.Text.Json.Serialization;

namespace VehicleRentalAPI.ValueObjects.Request;

public class UpdateVehicleRequest
{
    [JsonPropertyName("Placa")]
    public string Plate { get; set; }
}
