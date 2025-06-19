namespace VehicleRental.Application.DTOs.Rent;

public class CompleteRentRequestDTO
{
    public required string RentIdentifier { get; set; }
    public required DateTime ReturnDate { get; set; }
}
