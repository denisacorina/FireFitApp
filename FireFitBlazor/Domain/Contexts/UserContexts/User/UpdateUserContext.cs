using System;
using System.Threading.Tasks;
using FireFitBlazor.Domain.Models;
using FireFitBlazor.Domain.Resources;
using FireFitBlazor.Domain.Interfaces.Gateways.User;
using FireFitBlazor.Domain.ContextInterfaces.UserContexts.User;
using static FireFitBlazor.Domain.Enums.FoodTrackingEnums;

public class UpdateUserContext : IUpdateUserContext
{
    private readonly IUpdateUserGateway _updateUserGateway;

    public UpdateUserContext(IUpdateUserGateway updateUserGateway)
    {
        _updateUserGateway = updateUserGateway ?? throw new ArgumentNullException(nameof(updateUserGateway));
    }

     public async Task<bool> Execute(string userId, string email, string name, int age, bool isMale, int height, decimal startingWeight, decimal targetWeight, WeightChangeType changeType, ActivityLevel activityLevel, List<DietaryPreference> dietaryPreferences, List<WorkoutType> workoutTypes, string profilePicturePath, ExperienceLevel fitnessExperience)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException(Messages.Error_InvalidId);

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException(Messages.Error_EmptyEmail);

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException(Messages.Error_EmptyFirstName);

        var existingUser = await _updateUserGateway.GetUserById(userId);

        if (existingUser == null)
            throw new InvalidOperationException(Messages.Error_UserNotFound);
     
        existingUser = existingUser.Update(
            userId: userId,
            email: email,
            name: name,
            age: age,
            isMale: isMale ? true : false,
            height: height,
            startingWeight: startingWeight,
            targetWeight:targetWeight,
            changeType: changeType,
            activityLevel: activityLevel,
            dietaryPreferences: dietaryPreferences,
            workoutTypes: workoutTypes,
            profilePicturePath: profilePicturePath ?? String.Empty,
            fitnessExperience: fitnessExperience
        );


        await _updateUserGateway.UpdateAsync(existingUser);

        return true;
    }
}
