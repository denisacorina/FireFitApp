using System.ComponentModel.DataAnnotations;

namespace FireFitBlazor.Domain.Models
{
    public sealed class BodyMeasurement
    {
        [Key]
        public Guid MeasurementId { get; set; }
        public string UserId { get; set; }
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

        public BodyMeasurement() { }

        public static BodyMeasurement Create(
            string userId,
            decimal? weight = null,
            decimal? bodyFatPercentage = null,
            decimal? chest = null,
            decimal? waist = null,
            decimal? hips = null,
            decimal? leftArm = null,
            decimal? rightArm = null,
            decimal? leftThigh = null,
            decimal? rightThigh = null,
            decimal? leftCalf = null,
            decimal? rightCalf = null,
            string? notes = null)
        {
            return new BodyMeasurement
            {
                MeasurementId = Guid.NewGuid(),
                UserId = userId,
                MeasurementDate = DateTime.UtcNow,
                Weight = weight,
                BodyFatPercentage = bodyFatPercentage,
                Chest = chest,
                Waist = waist,
                Hips = hips,
                LeftArm = leftArm,
                RightArm = rightArm,
                LeftThigh = leftThigh,
                RightThigh = rightThigh,
                LeftCalf = leftCalf,
                RightCalf = rightCalf,
                Notes = notes
            };
        }

        public void Update(
            decimal? weight = null,
            decimal? bodyFatPercentage = null,
            decimal? chest = null,
            decimal? waist = null,
            decimal? hips = null,
            decimal? leftArm = null,
            decimal? rightArm = null,
            decimal? leftThigh = null,
            decimal? rightThigh = null,
            decimal? leftCalf = null,
            decimal? rightCalf = null,
            string? notes = null)
        {
            Weight = weight ?? Weight;
            BodyFatPercentage = bodyFatPercentage ?? BodyFatPercentage;
            Chest = chest ?? Chest;
            Waist = waist ?? Waist;
            Hips = hips ?? Hips;
            LeftArm = leftArm ?? LeftArm;
            RightArm = rightArm ?? RightArm;
            LeftThigh = leftThigh ?? LeftThigh;
            RightThigh = rightThigh ?? RightThigh;
            LeftCalf = leftCalf ?? LeftCalf;
            RightCalf = rightCalf ?? RightCalf;
            Notes = notes ?? Notes;
            MeasurementDate = DateTime.UtcNow;
        }
    }
} 