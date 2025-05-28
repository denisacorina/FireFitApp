namespace FireFitBlazor.Domain.Models
{
    public class MealIngredient
    {
        public Guid MealIngredientId { get; set; }
        public Guid MealId { get; set; }
        public string IngredientName { get; set; } = string.Empty;
        public float QuantityGrams { get; set; }
    }
}
