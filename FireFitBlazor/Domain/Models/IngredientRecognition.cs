
using System.ComponentModel.DataAnnotations;

namespace FireFitBlazor.Domain.Models
{
    public sealed class IngredientRecognition
    {
        [Key]
        public Guid RecognitionId { get; set; }
        public string UserId { get; set; }
        public string ImagePath { get; set; }
        public List<Ingredient> RecognizedIngredients { get; set; }
        public DateTime Timestamp { get; set; }

        public static IngredientRecognition Create(string userId, string imagePath, List<Ingredient> recognizedIngredients)
        {
            return new IngredientRecognition
            {
                RecognitionId = Guid.NewGuid(),
                UserId = userId,
                ImagePath = imagePath,
                RecognizedIngredients = recognizedIngredients,
                Timestamp = DateTime.UtcNow
            };
        }
    }
}
