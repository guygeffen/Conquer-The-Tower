using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
                    List<Tower> towers, double gameRadius,
                    double timeLimitInMinutes)
        {
            _centralLocation = centralLocation;
            _player = new Player(_centralLocation, playerHealth, playerWeapon);
            _towers = new List<Tower>();
            _towers.AddRange(towers);
            _startTime = GetCurrentTimeInMinutes(); // Capture start time
            _timeLimitInMinutes = timeLimitInMinutes;
        }

        public List<Tower> GetTowers() 
        {
            return _towers;
        }


        public Player GetPlayer()
        {
            return _player;
        }

        public static async Task<Game> CreateAsync(Location centralLocation, double playerHealth,
                    int numTowers, double gameRadius,
                    double timeLimitInMinutes)
        {
           
            List<Tower> towers = new List<Tower>();
            double radarRadius = (gameRadius / numTowers) * 0.5;
            double towerHealth = playerHealth / numTowers;
            for (int i = 0; i < numTowers; i++)
            {
                Weapon towerWeapon = new Weapon(radarRadius,0,0);
                Tower tower = await CreateRandomTowerAsync(centralLocation, towerHealth, towerWeapon, gameRadius, radarRadius);

                towers.Add(tower);
            }
            Weapon playerWeapon = new Weapon(radarRadius, 0, 0);
            return new Game(centralLocation, playerHealth, playerWeapon, towers,gameRadius, timeLimitInMinutes);
        }

        private static async Task<Tower> CreateRandomTowerAsync(Location center, double health, Weapon weapon, double radius, double radarRadius)
        {
            Random random = new Random();
            double randomAngle = random.NextDouble() * 360; // Generate random angle in degrees
            double distance = random.NextDouble() * radius; // Generate random distance within radius

            double newLatitude = center.Latitude + Math.Cos(randomAngle * Math.PI / 180) * (distance / 111111); // Convert distance to latitude offset
            double newLongitude = center.Longitude + Math.Sin(randomAngle * Math.PI / 180) * (distance / (111111 * Math.Cos(center.Latitude * Math.PI / 180))); // Convert distance to longitude offset

            ElevationService elevationService = new ElevationService();
            double? newAltitude = await elevationService.GetElevationAsync(newLatitude, newLongitude);
            newAltitude = newAltitude ?? center.Altitude; // Use center altitude if no data available

            Location newLocation = new Location(newLatitude, newLongitude, newAltitude.Value);
            return new Tower(newLocation, health, weapon, radarRadius, towerTurningSpeed, towerAimingTime);
        }

        public GameResult Update(Location currentPlayerLocation)
        {
            _player.Location = currentPlayerLocation; // Update player location based on GPS

            foreach (Tower tower in _towers)
            {
                tower.Attack(_player); // Towers attack the player
            }

            // Check for win/lose conditions (e.g., player health, remaining towers, time limit)
            double towerHealth = GetTowersHealth();
            double playerHealth = _player.Health;
            bool gameEnded = false;
            if (IsTimeLimitReached() || towerHealth <= 0 || playerHealth<=0)
            {
                gameEnded = true;
            }

            return new GameResult(gameEnded, playerHealth, towerHealth);
        }

        private double GetTowersHealth()
        {
            double towerHealth=0;
            foreach(Tower tower in _towers)
            {
                towerHealth += tower.Health;
            }
            return towerHealth;
        }



        private double GetCurrentTimeInMinutes()
        {
            DateTime now = DateTime.UtcNow; // Get current time in UTC
            return (now.Hour * 60) + now.Minute + (now.Second / 60.0); // Convert to total minutes
        }

        public double GetTimeLeftInMinutes()
        {
            double currentTimeInMinutes = GetCurrentTimeInMinutes();
            return _timeLimitInMinutes - (currentTimeInMinutes - _startTime);
        }

        private bool IsTimeLimitReached()
        {
           
            return GetTimeLeftInMinutes()<=0;
        }
    }

    public class GameResult
    {
        private bool _gameEnded;
        public enum Leader
        {
            player,
            computer,
            tie
        }


        private readonly double _playerScore;

        private readonly double _towerScore;

        public GameResult(bool gameEnded, double playerScore, double towerScore)
        {
            _gameEnded = gameEnded;
            _playerScore = playerScore;
            _towerScore = towerScore;
        }

        public double PlayerScore => _playerScore;

        public double TowerScore => _towerScore;

        public bool IsGameEnded() 
        {
            return _gameEnded;
        }
        public Leader GetLeader()
        {
            Leader result = Leader.tie;
            if (PlayerScore> TowerScore)
            {
                result = Leader.player;
            }
            else if(TowerScore> PlayerScore)
            {
                result = Leader.computer;
            }
            return result;
        }

    }

}