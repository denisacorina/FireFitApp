namespace FireFit.Shared.DTOs;

public class IngredientDto
{
    public string Name { get; set; } = string.Empty;
    public float Calories { get; set; }
    public float Protein { get; set; }
    public float Fat { get; set; }
    public float Carbs { get; set; }
    public float QuantityGrams { get; set; }
}

