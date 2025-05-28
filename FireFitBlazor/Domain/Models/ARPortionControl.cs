

using System.ComponentModel.DataAnnotations;

namespace FireFitBlazor.Domain.Models
{
    public sealed class ARPortionControl
    {
        [Key]
        public Guid PortionControlId { get; set; }
        public string UserId { get; set; }
        public string FoodName { get; set; }
        public decimal SuggestedPortionSize { get; set; }
        public DateTime Timestamp { get; set; }

        private ARPortionControl() { }

        public static ARPortionControl Create(string userId, string foodName, decimal portionSize)
        {
            return new ARPortionControl
            {
                PortionControlId = Guid.NewGuid(),
                UserId = userId,
                FoodName = foodName,
                SuggestedPortionSize = portionSize,
                Timestamp = DateTime.UtcNow
            };
        }
    }
}
