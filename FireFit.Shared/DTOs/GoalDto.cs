using FireFit.Shared.Enums;

namespace FireFit.Shared.DTOs;

public class GoalDto
{
    public string? Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public GoalType Type { get; set; }
    public int CalorieGoal { get; set; }
    public int ProteinGoal { get; set; }
    public int CarbGoal { get; set; }
    public int FatGoal { get; set; }
    public bool IntermittentFasting { get; set; }
    public int FastingWindowHours { get; set; }
    public decimal? TargetWeight { get; set; }
    public decimal? TargetBodyFatPercentage { get; set; }
    public DateTime? TargetDateUtc { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? CompletedAtUtc { get; set; }
}
