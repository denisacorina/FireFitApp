using FireFitBlazor.Application.Services;

namespace FireFitBlazor.Domain.Models;

public class DetectedIngredient
{
    public string Name { get; set; }
    public float X { get; set; } // As percent of width
    public float Y { get; set; } // As percent of height
    public string IconUrl { get; set; }
    public float Width { get; set; }
    public float Height { get; set; }
    public float Calories { get; set; } 
    public float Protein { get; set; } 
    public float Fat { get; set; } 
    public float Carbs { get; set; } 
    public float QuantityGrams { get; set; } = 100; // default to 100g

    public DetectedIngredient(string name, float x, float y, float width, float height, string iconUrl)
    {
        Name = name;
        X = x;
        Y = y;
        Width = width;
        Height = height;
        IconUrl = iconUrl;
    }
}