using CttApp;
using System;

/// <summary>
/// Class for calculating calories burned based on user profile and activities.
/// </summary>
public class CalorieCalculator
{
    /// <summary>
    /// Gets the user profile associated with the calorie calculator.
    /// </summary>
    public UserProfile User { get; private set; }

    /// <summary>
    /// Initializes a new instance of the CalorieCalculator class with the specified user profile.
    /// </summary>
    /// <param name="user">The user profile.</param>
    public CalorieCalculator(UserProfile user)
    {
        User = user;
    }

    /// <summary>
    /// Calculates the calories burned during rest for a given duration.
    /// </summary>
    /// <param name="restingSeconds">The duration of rest in seconds.</param>
    /// <returns>The number of calories burned during rest.</returns>
    public double CalculateRestingCalories(int restingSeconds)
    {
        double dailyRestingCalories = CalculateDailyRestingCalories();  // Daily resting calories
        double restingCaloriesPerSecond = dailyRestingCalories / 86400;  // Convert daily calories to per second
        return restingCaloriesPerSecond * restingSeconds;  // Total resting calories over the specified period
    }

    /// <summary>
    /// Calculates the daily resting metabolic rate (BMR) calories.
    /// </summary>
    /// <returns>The daily BMR calories.</returns>
    private double CalculateDailyRestingCalories()
    {
        // BMR calculation based on Mifflin-St Jeor Equation
        if (User.Gender.ToLower() == "male")
        {
            return 10 * User.Weight + 6.25 * User.Height - 5 * User.Age + 5;
        }
        else
        {
            return 10 * User.Weight + 6.25 * User.Height - 5 * User.Age - 161;
        }
    }

    /// <summary>
    /// Calculates the calories burned based on distance and time.
    /// </summary>
    /// <param name="newLoc">The new location.</param>
    /// <param name="oldLoc">The old location.</param>
    /// <param name="timeSeconds">The time in seconds.</param>
    /// <returns>The number of calories burned.</returns>
    public double CalculateCaloriesBurned(Location newLoc, Location oldLoc, int timeSeconds)
    {
        double distanceKm = newLoc.DistanceTo(oldLoc) / 1000;
        if (distanceKm == 0)
        {
            return CalculateRestingCalories(timeSeconds);
        }
        else
        {
            double speedKmh = (distanceKm / timeSeconds) * 3600; // Convert km/s to km/h
            double met = GetMetValue(speedKmh);
            double caloriesPerSecond = met * User.Weight * 3.5 / 200;
            return caloriesPerSecond * timeSeconds;
        }
    }

    /// <summary>
    /// Determines the MET value based on speed.
    /// </summary>
    /// <param name="speedKmh">The speed in kilometers per hour.</param>
    /// <returns>The MET value.</returns>
    private double GetMetValue(double speedKmh)
    {
        if (speedKmh < 5)
            return 2.0; // Slow walking
        else if (speedKmh < 7.5)
            return 3.5; // Normal walking
        else
            return 5.0; // Brisk walking
    }
}
