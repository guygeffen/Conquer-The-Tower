using System;
using System.Collections.Generic;

namespace CttApp
{
    public class Game
    {
        private Location _centralLocation;
        private Player _player;
        private List<Tower> _towers;
        private double _startTime;
        private double _timeLimitInMinutes;
        static private double towerAimingTime = 1;
        static private double towerTurningSpeed = 1;

        public Game(Location centralLocation, double playerHealth, Weapon playerWeapon,
                    int numTowers, double towerHealth, Weapon towerWeapon, double gameRadius,
                    double timeLimitInMinutes)
        {
            _centralLocation = centralLocation;
            _player = new Player(_centralLocation, playerHealth, playerWeapon);
            _towers = new List<Tower>();
            double radarRadius = (gameRadius / numTowers) * 0.5;

            for (int i = 0; i < numTowers; i++)
            {
                _towers.Add(CreateRandomTower(_centralLocation, towerHealth, towerWeapon, gameRadius, radarRadius));
            }

            _startTime = GetCurrentTimeInMinutes(); // Capture start time
            _timeLimitInMinutes = timeLimitInMinutes;
        }

        public void Update(Location currentPlayerLocation)
        {
            _player.Location = currentPlayerLocation; // Update player location based on GPS

            // Handle player attack (if applicable)
            // ... (implement player targeting and attack logic)

            foreach (Tower tower in _towers)
            {
                tower.Attack(_player); // Towers attack the player
            }

            // Check for win/lose conditions (e.g., player health, remaining towers, time limit)
            if (IsTimeLimitReached())
            {
                // Time limit reached, end the game
                // ... (implement game over logic, display message, etc.)
            }
        }

        private Tower CreateRandomTower(Location center, double health, Weapon weapon, double radius, double radarRadius)
        {
            Random random = new Random();
            double randomAngle = random.NextDouble() * 360; // Generate random angle in degrees
            double distance = random.NextDouble() * radius; // Generate random distance within radius

            double newLatitude = center.Latitude + Math.Cos(randomAngle) * (distance / 111111); // Convert distance to latitude offset (assuming Earth's radius)
            double newLongitude = center.Longitude + Math.Sin(randomAngle) * (distance / (111111 * Math.Cos(center.Latitude))); // Convert distance to longitude offset

            return new Tower(new Location(newLatitude, newLongitude), health, weapon, radarRadius, towerTurningSpeed, towerAimingTime);
        }

        private double GetCurrentTimeInMinutes()
        {
            DateTime now = DateTime.UtcNow; // Get current time in UTC
            return (now.Hour * 60) + now.Minute + (now.Second / 60.0); // Convert to total minutes
        }

        private bool IsTimeLimitReached()
        {
            double currentTimeInMinutes = GetCurrentTimeInMinutes();
            return currentTimeInMinutes - _startTime >= _timeLimitInMinutes;
        }
    }

}