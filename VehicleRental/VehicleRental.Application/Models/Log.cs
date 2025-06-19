namespace VehicleRental.Application.Models;

public class Log
{
    public int Id { get; set; }
    public string? UserAgent { get; set; }
    public DateTime? RequestDate { get; set; }
    public string? RequestMethod { get; set; }
    public string? SerializedRequest { get; set; }
    public DateTime? ResponseDate { get; set; }
    public string? SerializedResponse { get; set; }
    public int? ResponseStatusCode { get; set; }
    public long? TotalExecutionTime { get; set; }
    public string? IPAddress { get; set; }
}
