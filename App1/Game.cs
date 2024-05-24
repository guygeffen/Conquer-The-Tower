using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CttApp
{
    public class Game
    {
        private readonly Location _centralLocation;
        private readonly Player _player;
        private readonly List<Tower> _towers;
        private double _startTime;
        private readonly double _timeLimitInMinutes;
        static private readonly double towerAimingTime = 1;
        static private readonly double towerAngleChangeStep = 2;
        

        public Game(Location centralLocation, double playerHealth, Weapon playerWeapon, UserProfile user,
                    List<Tower> towers, double gameRadius,
                    double timeLimitInMinutes)
        {
            _centralLocation = centralLocation;
            _player = new Player(_centralLocation, playerHealth, playerWeapon, user);
            _towers = new List<Tower>();
            _towers.AddRange(towers);
            _timeLimitInMinutes = timeLimitInMinutes;
        }

        public void StartGame()
        {
            _startTime = GetCurrentTimeInMinutes(); // Capture start time
            _player.startCalorieTracking();
        }
        

        public List<Tower> GetTowers() 
        {
            return _towers;
        }


        public Player GetPlayer()
        {
            return _player;
        }

        public static async Task<Game> CreateAsync(Location centralLocation, double playerHealth, UserProfile user,
                    int numTowers, double gameRadius,
                    double timeLimitInMinutes)
        {
           
            List<Tower> towers = new List<Tower>();
            double towerRange = gameRadius; //(gameRadius *0.5/ numTowers) * 0.5;
            double towerHealth = playerHealth / numTowers;
            Random random = new Random();
            double randomAngle = random.NextDouble() * 360;
            double randomInclination = random.NextDouble() * 90;
            Weapon playerWeapon = new Weapon(towerRange*1.1, randomAngle, randomInclination);
            for (int i = 0; i < numTowers; i++)
            {
                randomAngle = random.NextDouble() * 360;
                randomInclination = random.NextDouble() * 90;
                Weapon towerWeapon = new Weapon(towerRange, randomAngle, randomInclination);
                Tower tower = await CreateRandomTowerAsync(i,centralLocation, towerHealth, towerWeapon, gameRadius, towerRange*1.25);

                towers.Add(tower);
            }
           
            return new Game(centralLocation, playerHealth, playerWeapon, user, towers,gameRadius, timeLimitInMinutes);
        }


        private static async Task<Tower> CreateRandomTowerAsync(int index,Location center, double health, Weapon weapon, double radius, double radarRadius)
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
            return new Tower(index,newLocation, health, weapon, radarRadius, towerAngleChangeStep, towerAimingTime);
        }

        public GameResult Update(Location currentPlayerLocation)
        {
            Location previousLocation = _player.Location;
            _player.Location = currentPlayerLocation; // Update player location based on GPS
            

            List<ShootingEntityHitResults> hitResults = new List<ShootingEntityHitResults>();
            foreach (Tower tower in _towers)
            {
                hitResults.Add(tower.Attack(_player)); // Towers attack the player
            }

            // Check for win/lose conditions (e.g., player health, remaining towers, time limit)
            double towerHealth = GetTowersHealth();
            double playerHealth = _player.Health;
            bool gameEnded = false;
            if (IsTimeLimitReached() || towerHealth <= 0 || playerHealth<=0)
            {
                gameEnded = true;
            }

            return new GameResult(gameEnded, playerHealth, towerHealth, hitResults);
        }

        public double GetTowersHealth()
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
        private readonly bool _gameEnded;
        public enum Leader
        {
            player,
            computer,
            tie
        }


        private readonly double _playerScore;

        private readonly double _towerScore;
        private readonly List<ShootingEntityHitResults> _hitResults;

        public GameResult(bool gameEnded, double playerScore, double towerScore, List<ShootingEntityHitResults> hitResults)
        {
            _hitResults = hitResults;
            _gameEnded = gameEnded;
            _playerScore = playerScore;
            _towerScore = towerScore;
        }

        public double PlayerScore => _playerScore;

        public double TowerScore => _towerScore;

        public List<ShootingEntityHitResults> HitResults => _hitResults;

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