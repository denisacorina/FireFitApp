using System.ComponentModel.DataAnnotations;

namespace FireFitBlazor.Domain.Models
{
    public sealed class BodyMeasurement
    {
        [Key]
        public Guid MeasurementId { get; private init; }
        public string UserId { get; private init; }
        public DateTime MeasurementDate { get; private init; }
        public decimal? Weight { get; private init; }
        public decimal? BodyFatPercentage { get; private init; }
        public decimal? Chest { get; private init; }
        public decimal? Waist { get; private init; }
        public decimal? Hips { get; private init; }
        public decimal? LeftArm { get; private init; }
        public decimal? RightArm { get; private init; }
        public decimal? LeftThigh { get; private init; }
        public decimal? RightThigh { get; private init; }
        public decimal? LeftCalf { get; private init; }
        public decimal? RightCalf { get; private init; }
        public string? Notes { get; private init; }

        private BodyMeasurement() { }

        private BodyMeasurement(
            Guid measurementId,
            string userId,
            DateTime measurementDate,
            decimal? weight,
            decimal? bodyFatPercentage,
            decimal? chest,
            decimal? waist,
            decimal? hips,
            decimal? leftArm,
            decimal? rightArm,
            decimal? leftThigh,
            decimal? rightThigh,
            decimal? leftCalf,
            decimal? rightCalf,
            string? notes)
        {
            MeasurementId = measurementId;
            UserId = userId;
            MeasurementDate = measurementDate;
            Weight = weight;
            BodyFatPercentage = bodyFatPercentage;
            Chest = chest;
            Waist = waist;
            Hips = hips;
            LeftArm = leftArm;
            RightArm = rightArm;
            LeftThigh = leftThigh;
            RightThigh = rightThigh;
            LeftCalf = leftCalf;
            RightCalf = rightCalf;
            Notes = notes;
        }

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
            return new BodyMeasurement(
                measurementId: Guid.NewGuid(),
                userId: userId,
                measurementDate: DateTime.UtcNow,
                weight: weight,
                bodyFatPercentage: bodyFatPercentage,
                chest: chest,
                waist: waist,
                hips: hips,
                leftArm: leftArm,
                rightArm: rightArm,
                leftThigh: leftThigh,
                rightThigh: rightThigh,
                leftCalf: leftCalf,
                rightCalf: rightCalf,
                notes: notes
            );
        }

        public BodyMeasurement Update(
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
            return new BodyMeasurement(
                measurementId: MeasurementId,
                userId: UserId,
                measurementDate: DateTime.UtcNow,
                weight: weight ?? Weight,
                bodyFatPercentage: bodyFatPercentage ?? BodyFatPercentage,
                chest: chest ?? Chest,
                waist: waist ?? Waist,
                hips: hips ?? Hips,
                leftArm: leftArm ?? LeftArm,
                rightArm: rightArm ?? RightArm,
                leftThigh: leftThigh ?? LeftThigh,
                rightThigh: rightThigh ?? RightThigh,
                leftCalf: leftCalf ?? LeftCalf,
                rightCalf: rightCalf ?? RightCalf,
                notes: notes ?? Notes
            );
        }
    }
}
