using CttApp;
using System;

public class CalorieCalculator
{
   
    public UserProfile User { get; private set; }

    // Private constructor to prevent instantiation from outside
    public CalorieCalculator(UserProfile user)
    {
        User = user;
    }


    // Calculate calories burned during rest for a given duration
    public double CalculateRestingCalories(int restingSeconds)
    {
        double dailyRestingCalories = CalculateDailyRestingCalories();  // Daily resting calories
        double restingCaloriesPerSecond = dailyRestingCalories / 86400;  // Convert daily calories to per second
        return restingCaloriesPerSecond * restingSeconds;  // Total resting calories over the specified period
    }

    // Calculate resting metabolic rate (BMR) calories per day
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

    // Calculate calories burned based on distance and time
    public double CalculateCaloriesBurned(Location newLoc, Location oldLoc, int timeSeconds)
    {
        double distanceKm = newLoc.DistanceTo(oldLoc)/1000;
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

    // Determine MET value based on speed
    private double GetMetValue(double speedKmh)
    {
        if (speedKmh < 5)
            return 2.0; // slow walking
        else if (speedKmh < 7.5)
            return 3.5; // normal walking
        else
            return 5.0; // brisk walking
    }

    
}
