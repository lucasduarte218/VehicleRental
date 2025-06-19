using System.Text.Json.Serialization;

namespace VehicleRentalAPI.ValueObjects.Request
{
    public class UpdateCourierRequest
    {
        [JsonPropertyName("imagem_cnh")]
        public string DriverLicenseImageBase64 { get; set; }
    }
}
