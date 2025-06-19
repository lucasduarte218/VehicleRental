using System.Text.Json.Serialization;
using VehicleRental.Domain.Enums;

namespace VehicleRentalAPI.ValueObjects.Request;

public class InsertCourierRequest
{
    [JsonPropertyName("identificador")]
    public string Identifier { get; set; }

    [JsonPropertyName("nome")]
    public string Name { get; set; }

    [JsonPropertyName("cnpj")]
    public string TaxNumber { get; set; }

    [JsonPropertyName("data_nascimento")]
    public DateTime BirthDate { get; set; }

    [JsonPropertyName("numero_cnh")]
    public string DriverLicenseNumber { get; set; }

    [JsonPropertyName("tipo_cnh")]
    public DriverLicenseType DriverLicenseType { get; set; }

    [JsonPropertyName("imagem_CNH")]
    public string DriverLicenseImageBase64 { get; set; }
}
