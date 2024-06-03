
using System;

namespace CttApp
{
    public class Player : Entity
    {
        public UserProfile User { get; private set; }
        private readonly CalorieCalculator calorieCalculator;
      
        private long lastUpdate = 0;
        public double CaloriesBurned { get; private set; }
        // Additional player-specific properties or methods (e.g., movement)
        public Player(Location location, double health, Weapon weapon,UserProfile user) : base($"{user.Name}", location, health, weapon) 
        {
            User = user;
            calorieCalculator = new CalorieCalculator(user);
            CaloriesBurned = 0;
           
        }

        protected override void LocationChanged(Location oldLocation, Location newLocation)
        {
            if (lastUpdate!=0 && GetTimeInSecondsSinceUpdate()>2)
            {
                CaloriesBurned += calorieCalculator.CalculateCaloriesBurned(newLocation, oldLocation, GetTimeInSecondsSinceUpdate());
                StartCalorieTracking();
            }
            
            base.LocationChanged(oldLocation, newLocation);
        }
        public void StartCalorieTracking()
        {
            lastUpdate = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }

        protected int GetTimeInSecondsSinceUpdate()
        {
            return (int)(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - lastUpdate) / 1000;
        }

    }

}