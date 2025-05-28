
using System.ComponentModel.DataAnnotations;
using FireFitBlazor.Domain.ValueObjects;

namespace FireFitBlazor.Domain.Models
{
    public sealed class BarcodeProduct
    {
        [Key]
        public string BarcodeNumber { get; set; }
        public string ProductName { get; set; }
        public NutritionalInfo Nutrition { get; set; }

     

        public static BarcodeProduct Create(string barcodeNumber, string productName, int calories, float proteins, float carbs, float fats)
        {
            return new BarcodeProduct
            {
                BarcodeNumber = barcodeNumber,
                ProductName = productName,
                Nutrition = NutritionalInfo.Create(calories, proteins, carbs, fats)
            };
        }
    }
}
