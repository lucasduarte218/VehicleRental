using System.Text.Json.Serialization;

namespace VehicleRentalAPI.ValueObjects.Request;

public class CompleteRentRequest
{
    [JsonPropertyName("data_devolucao")]
    public required DateTime ReturnDate { get; set; }
}
