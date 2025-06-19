namespace VehicleRental.Notify.Entities;

public record VehicleRegisteredEvent(string Identifier,
                                        string Plate,
                                        int Year,
                                        string Model);
