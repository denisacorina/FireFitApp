

namespace FireFitBlazor.Domain.Enums
{
    public class FoodTrackingEnums
    {
        public enum MealType
        {
            Breakfast,
            Lunch,
            Dinner,
            Snack
        }

        public enum DietaryPreference
        {
            None,
            Vegan,
            Vegetarian,
            Keto,
            Paleo,
            GlutenFree,
            LactoseIntolerant
        }

        public enum AchievementType
        {
            Streak,
            CalorieGoal,
            ProteinGoal,
            WeightLoss,
            Workout
        }

        public enum WeightChangeType
        {
            Maintain,
            Lose,
            Gain
        }

        public enum GoalIntensity 
        { 
            Moderate,
            Extreme,
            Slow 
        }

        public enum ActivityLevel
        {
            Sedentary,      
            Light,      
            Moderate,     
            Active,      
            VeryActive   
        }

        public enum Gender
        {
            Male,
            Female
        }

        public enum ExperienceLevel
        {
            Beginner,
            Intermediate,
            Advanced,
            Expert
        }

        public enum WorkoutType
        {
            None,
            Strength,
            Cardio,
            Flexibility,
            HIIT,
            CrossFit,
            Yoga
        }
    }
}
