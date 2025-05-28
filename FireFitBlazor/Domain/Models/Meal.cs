namespace FireFitBlazor.Domain.Models
{
    public class Meal
    {
        public Guid MealId { get; set; }
        public string UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public List<MealIngredient> Ingredients { get; set; } = new();
    }

}
