namespace VehicleRental.Infrastructure.Settings;

public class AwsS3Settings
{
    public required string BucketName { get; set; }
    public required string Secret { get; set; }
    public required string Key { get; set; }
}
