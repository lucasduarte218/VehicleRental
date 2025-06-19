namespace VehicleRental.Application.Events;

public record VehicleRegisteredEvent(string Identifier,
                                        string Plate,
                                        int Year,
                                        string Model);
