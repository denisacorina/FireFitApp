namespace FireFitBlazor.Application.DTOs
{
    public class ProgressDto
    {
        public Guid ProgressId { get; set; }
        public Guid UserId { get; set; }
        public decimal StartingWeight { get; set; }
        public decimal CurrentWeight { get; set; }
        public decimal? StartingBodyFatPercentage { get; set; }
        public decimal? CurrentBodyFatPercentage { get; set; }
        public decimal WeightChange { get; set; }
        public decimal? BodyFatChange { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? LastMeasurementDate { get; set; }
        public string? Notes { get; set; }
    }

    public class BodyMeasurementDto
    {
        public Guid MeasurementId { get; set; }
        public Guid UserId { get; set; }
        public DateTime MeasurementDate { get; set; }
        public decimal? Weight { get; set; }
        public decimal? BodyFatPercentage { get; set; }
        public decimal? Chest { get; set; }
        public decimal? Waist { get; set; }
        public decimal? Hips { get; set; }
        public decimal? LeftArm { get; set; }
        public decimal? RightArm { get; set; }
        public decimal? LeftThigh { get; set; }
        public decimal? RightThigh { get; set; }
        public decimal? LeftCalf { get; set; }
        public decimal? RightCalf { get; set; }
        public string? Notes { get; set; }
    }
} 