using FireFit.Shared.Enums;

namespace FireFit.Shared.DTOs;

public class FoodLogDto
{
    public string? Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string FoodName { get; set; } = string.Empty;
    public int Calories { get; set; }
    public float Proteins { get; set; }
    public float Carbs { get; set; }
    public float Fats { get; set; }
    public MealType MealType { get; set; }
    public DateTime TimestampUtc { get; set; } = DateTime.UtcNow;
}

