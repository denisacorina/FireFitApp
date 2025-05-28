using System.Resources;

namespace FireFitBlazor.Domain.Resources
{
    public static class Messages
    {
        private static readonly ResourceManager ResourceManager = new ResourceManager("FireFitBlazor.Domain.Resources.Messages", typeof(Messages).Assembly);

        // Error Messages
        public static string Error_NullEntity => ResourceManager.GetString("Error_NullEntity");
        public static string Error_InvalidId => ResourceManager.GetString("Error_InvalidId");
        public static string Error_InvalidCalorieLogId => ResourceManager.GetString("Error_InvalidCalorieLogId");
        public static string Error_InvalidFoodId => ResourceManager.GetString("Error_InvalidFoodId");
        public static string Error_EntityNotFound => ResourceManager.GetString("Error_EntityNotFound");
        public static string Error_DatabaseOperationFailed => ResourceManager.GetString("Error_DatabaseOperationFailed");
        public static string Error_ValidationFailed => ResourceManager.GetString("Error_ValidationFailed");
        public static string Error_InvalidDate => ResourceManager.GetString("Error_InvalidDate");
        public static string Error_InvalidDateRange => ResourceManager.GetString("Error_InvalidDateRange");
        public static string Error_DuplicateDate => ResourceManager.GetString("Error_DuplicateDate");
        public static string Error_DuplicateFoodLog => ResourceManager.GetString("Error_DuplicateFoodLog");
        public static string Error_EmptyName => ResourceManager.GetString("Error_EmptyName");
        public static string Error_DuplicateFood => ResourceManager.GetString("Error_DuplicateFood");
        public static string Error_FoodInUse => ResourceManager.GetString("Error_FoodInUse");
        public static string Error_DuplicateIngredient => ResourceManager.GetString("Error_DuplicateIngredient");
        public static string Error_DuplicateBarcode => ResourceManager.GetString("Error_DuplicateBarcode");
        public static string Error_DuplicateGoal => ResourceManager.GetString("Error_DuplicateGoal");
        public static string Error_DuplicateRecipe => ResourceManager.GetString("Error_DuplicateRecipe");
        public static string Error_DuplicateWorkout => ResourceManager.GetString("Error_DuplicateWorkout");
        public static string Error_DuplicateEmail => ResourceManager.GetString("Error_DuplicateEmail");

        // Success Messages
        public static string Success_EntityAdded => ResourceManager.GetString("Success_EntityAdded");
        public static string Success_EntityUpdated => ResourceManager.GetString("Success_EntityUpdated");
        public static string Success_EntityDeleted => ResourceManager.GetString("Success_EntityDeleted");
        public static string Success_OperationCompleted => ResourceManager.GetString("Success_OperationCompleted");

        public static string? Error_InvalidWeight { get; internal set; }
        public static string? Error_InvalidHeight { get; internal set; }
        public static string? Error_EmptyEmail { get; internal set; }
        public static string? Error_EmptyFirstName { get; internal set; }
    }
} 