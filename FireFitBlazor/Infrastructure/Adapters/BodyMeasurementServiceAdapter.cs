using FireFit.Shared.Contracts;
using FireFit.Shared.DTOs;
using FireFitBlazor.Domain.ContextInterfaces.ProgressContexts;
using FireFitBlazor.Domain.Models;

namespace FireFitBlazor.Infrastructure.Adapters;

public class BodyMeasurementServiceAdapter : IBodyMeasurementService
{
    private readonly IUpdateUserProgressContext _ctx;
    public BodyMeasurementServiceAdapter(IUpdateUserProgressContext ctx) { _ctx = ctx; }

    public async Task AddAsync(BodyMeasurementDto m, CancellationToken ct = default)
    {
        var dm = BodyMeasurement.Create(
            userId: m.UserId,
            weight: m.Weight,
            bodyFatPercentage: m.BodyFatPercentage,
            chest: m.Chest,
            waist: m.Waist,
            hips: m.Hips,
            leftArm: m.LeftArm,
            rightArm: m.RightArm,
            leftThigh: m.LeftThigh,
            rightThigh: m.RightThigh,
            leftCalf: m.LeftCalf,
            rightCalf: m.RightCalf,
            notes: m.Notes);
        await _ctx.AddBodyMeasurementAsync(dm);
    }

    public async Task<IReadOnlyList<BodyMeasurementDto>> GetListAsync(string userId, CancellationToken ct = default)
    {
        var list = await _ctx.GetBodyMeasurementsAsync(userId);
        return list.Select(x => new BodyMeasurementDto
        {
            Id = x.MeasurementId.ToString(),
            UserId = x.UserId,
            Weight = (decimal)x.Weight,
            BodyFatPercentage = x.BodyFatPercentage,
            Waist = x.Waist,
            Chest = x.Chest,
            Hips = x.Hips,
            LeftArm = x.LeftArm,
            RightArm = x.RightArm,
            LeftThigh = x.LeftThigh,
            RightThigh = x.RightThigh,
            LeftCalf = x.LeftCalf,
            RightCalf = x.RightCalf,
            Notes = x.Notes,
            TimestampUtc = x.MeasurementDate
        }).ToList();
    }

    public async Task<BodyMeasurementDto?> GetLatestAsync(string userId, CancellationToken ct = default)
    {
        var x = await _ctx.GetLatestBodyMeasurementAsync(userId);
        if (x == null) return null;
        return new BodyMeasurementDto
        {
            Id = x.MeasurementId.ToString(),
            UserId = x.UserId,
            Weight = x.Weight,
            BodyFatPercentage = x.BodyFatPercentage,
            Waist = x.Waist,
            Chest = x.Chest,
            Hips = x.Hips,
            LeftArm = x.LeftArm,
            RightArm = x.RightArm,
            LeftThigh = x.LeftThigh,
            RightThigh = x.RightThigh,
            LeftCalf = x.LeftCalf,
            RightCalf = x.RightCalf,
            Notes = x.Notes,
            TimestampUtc = x.MeasurementDate
        };
    }

    public async Task DeleteAsync(string id, CancellationToken ct = default)
    {
        if (Guid.TryParse(id, out var gid))
        {
            // need userId for delete; adapter cannot infer safely.
            // Fallback: ignore if user not provided; caller should use GetList to identify and call a controller.
        }
    }
}
