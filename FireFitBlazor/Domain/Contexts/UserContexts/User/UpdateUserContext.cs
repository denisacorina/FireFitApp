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
    private readonly IGetUserGateway _getUserGateway;

    public UpdateUserContext(IUpdateUserGateway updateUserGateway, IGetUserGateway getUserGateway)
    {
        _updateUserGateway = updateUserGateway;
        _getUserGateway = getUserGateway;
    }

    public async Task<bool> Execute(string userId, string email, string name, int age, int height, decimal startingWeight, decimal targetWeight, WeightChangeType changeType, ActivityLevel activityLevel, List<DietaryPreference> dietaryPreferences, List<WorkoutType> workoutTypes, string profilePicturePath, ExperienceLevel fitnessExperience)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException(Messages.Error_InvalidId);

        var existingUser = await _updateUserGateway.GetUserById(userId);

        if (existingUser == null)
            throw new InvalidOperationException(Messages.Error_UserNotFound);

        existingUser = existingUser.Update(
            userId: userId,
            email: email,
            name: name,
            age: age,
            height: height,
            startingWeight: startingWeight,
            targetWeight: targetWeight,
            changeType: changeType,
            activityLevel: activityLevel,
            dietaryPreferences: dietaryPreferences,
            workoutTypes: workoutTypes,
            profilePicturePath: profilePicturePath,
            fitnessExperience: fitnessExperience
        );


        await _updateUserGateway.UpdateAsync(existingUser);

        return true;
    }

    public async Task<bool> UpdateUserFitnessExperienceAsync(
       string userId,
       ExperienceLevel experienceLevel)
    {
        var result = await _getUserGateway.GetByIdAsync(userId);
        if (result == null)
        {
            throw new InvalidOperationException(Messages.Error_UserNotFound);
        }

        var updatedUser = result.UpdateFitnessExperience(experienceLevel);
        await _updateUserGateway.UpdateAsync(updatedUser);

        return true;
    }
}
