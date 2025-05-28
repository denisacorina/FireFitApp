namespace FoodObjectDetection_WebApi
{
    public class Train
    {
        static void Main(string[] args)
        {
            ObjectDetection.Train();
            Console.WriteLine("Retraining complete. Model saved");
        }
    }
}
