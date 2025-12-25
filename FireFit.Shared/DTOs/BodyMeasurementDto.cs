namespace FireFit.Shared.DTOs;

public class BodyMeasurementDto
{
    public string? Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public decimal? Weight { get; set; }
    public decimal? BodyFatPercentage { get; set; }
    public decimal? Waist { get; set; }
    public decimal? Chest { get; set; }
    public decimal? Hips { get; set; }
    public decimal? LeftArm { get; set; }
    public decimal? RightArm { get; set; }
    public decimal? LeftThigh { get; set; }
    public decimal? RightThigh { get; set; }
    public decimal? LeftCalf { get; set; }
    public decimal? RightCalf { get; set; }
    public string? Notes { get; set; }
    public DateTime TimestampUtc { get; set; } = DateTime.UtcNow;
}
