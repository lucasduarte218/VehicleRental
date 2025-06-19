namespace VehicleRental.Application.Models;

public class ExceptionLog
{
    public int Id { get; set; }
    public DateTime? OccurredAt { get; set; }
    public string? Message { get; set; }
    public string? StackTrace { get; set; }
    public string? Source { get; set; }
    public string? InnerException { get; set; }
    public string? RequestPath { get; set; }
    public string? RequestMethod { get; set; }
    public string? IPAddress { get; set; }
}
