using Android.Gms.Maps.Model;
using System;
using System.Collections.Generic;

namespace CttApp
{
    /// <summary>
    /// Represents a game with a central location, player, towers, and time limit.
    /// </summary>
    public class Game
    {
        private readonly Location _centralLocation;
        private readonly Player _player;
        private readonly List<Tower> _towers;
        private double _startTime;
        private readonly double _timeLimitInMinutes;

        /// <summary>
        /// Gets a value indicating whether the game has ended.
        /// </summary>
        public bool GameEnded { get; private set; }

        /// <summary>
        /// Initializes a new instance of the Game class.
        /// </summary>
        /// <param name="centralLocation">The central location of the game.</param>
        /// <param name="playerHealth">The health of the player.</param>
        /// <param name="playerWeapon">The weapon of the player.</param>
        /// <param name="user">The user profile.</param>
        /// <param name="towers">The list of towers.</param>
        /// <param name="gameRadius">The radius of the game area.</param>
        /// <param name="timeLimitInMinutes">The time limit of the game in minutes.</param>
        public Game(Location centralLocation, double playerHealth, Weapon playerWeapon, UserProfile user,
                    List<Tower> towers, double gameRadius,
                    double timeLimitInMinutes)
        {
            _centralLocation = centralLocation;
            _player = new Player(_centralLocation, playerHealth, playerWeapon, user);
            _towers = new List<Tower>();
            _towers.AddRange(towers);
            _timeLimitInMinutes = timeLimitInMinutes;
            GameEnded = false;
        }

        /// <summary>
        /// Starts the game by capturing the start time and initiating calorie tracking for the player.
        /// </summary>
        public void StartGame()
        {
            _startTime = GetCurrentTimeInMinutes(); // Capture start time
            _player.StartCalorieTracking();
        }

        /// <summary>
        /// Gets the list of towers in the game.
        /// </summary>
        /// <returns>The list of towers.</returns>
        public List<Tower> GetTowers()
        {
            return _towers;
        }

        /// <summary>
        /// Gets the player in the game.
        /// </summary>
        /// <returns>The player.</returns>
        public Player GetPlayer()
        {
            return _player;
        }

        /// <summary>
        /// Creates a new game instance with randomly placed towers.
        /// </summary>
        /// <param name="centralLocation">The central location of the game.</param>
        /// <param name="playerHealth">The health of the player.</param>
        /// <param name="user">The user profile.</param>
        /// <param name="numTowers">The number of towers.</param>
        /// <param name="gameRadius">The radius of the game area.</param>
        /// <param name="timeLimitInMinutes">The time limit of the game in minutes.</param>
        /// <param name="range">The range of the towers and player weapons.</param>
        /// <returns>A new instance of the Game class.</returns>
        public static Game Create(Location centralLocation, double playerHealth, UserProfile user,
                    int numTowers, double gameRadius,
                    double timeLimitInMinutes, int range)
        {
            List<Tower> towers = new List<Tower>();

            double towerHealth = playerHealth / numTowers;
            Random random = new Random();

            Weapon playerWeapon = new Weapon(range, 0, 0, GameConstants.PlayerRoundsPerMinute);
            for (int i = 0; i < numTowers; i++)
            {
                double randomAngle = random.NextDouble() * 360;
                double randomInclination = random.NextDouble() * 90;
                Weapon towerWeapon = new Weapon(range, randomAngle, randomInclination, GameConstants.TowerRoundsPerMinute);
                Tower tower = CreateRandomTower(i, centralLocation, towerHealth, towerWeapon, gameRadius, range);

                towers.Add(tower);
            }

            return new Game(centralLocation, playerHealth, playerWeapon, user, towers, gameRadius, timeLimitInMinutes);
        }

        /// <summary>
        /// Creates a tower at a random location within a given radius.
        /// </summary>
        /// <param name="index">The index of the tower.</param>
        /// <param name="center">The central location of the game.</param>
        /// <param name="health">The health of the tower.</param>
        /// <param name="weapon">The weapon of the tower.</param>
        /// <param name="radius">The radius within which the tower is placed.</param>
        /// <param name="radarRadius">The radar radius of the tower.</param>
        /// <returns>A new instance of the Tower class.</returns>
        private static Tower CreateRandomTower(int index, Location center, double health, Weapon weapon, double radius, double radarRadius)
        {
            Random random = new Random();
            double randomAngle = random.NextDouble() * 360; // Generate random angle in degrees
            double distance = random.NextDouble() * radius; // Generate random distance within radius

            LatLng towerLatLng = SphericalUtil.ComputeOffset(center.GetLatLng(), distance, randomAngle);

            Location newLocation = new Location(towerLatLng.Latitude, towerLatLng.Longitude);
            return new Tower(index, newLocation, health, weapon, radarRadius, GameConstants.towerAngleChangeStep, GameConstants.towerAimingTime);
        }

        /// <summary>
        /// Updates the game state based on the current player location.
        /// </summary>
        /// <param name="currentPlayerLocation">The current location of the player.</param>
        /// <returns>The result of the game update.</returns>
        public GameResult Update(Location currentPlayerLocation)
        {
            _player.Location = currentPlayerLocation; // Update player location based on GPS

            List<ShootingEntityHitResults> hitResults = new List<ShootingEntityHitResults>();
            foreach (Tower tower in _towers)
            {
                hitResults.Add(tower.Attack(_player)); // Towers attack the player
            }

            // Check for win/lose conditions (e.g., player health, remaining towers, time limit)
            double towerHealth = GetTowersHealth();
            double playerHealth = _player.Health;

            if (IsTimeLimitReached() || towerHealth <= 0 || playerHealth <= 0)
            {
                // Debugging information
                Console.WriteLine($"Player Health: {playerHealth}");
                Console.WriteLine($"Tower Health: {towerHealth}");

                GameEnded = true;
            }
            Console.WriteLine($"Time Left: {GetTimeLeftInMinutes()}");
            return new GameResult(GameEnded, playerHealth, towerHealth, hitResults);
        }

        /// <summary>
        /// Gets the total health of all towers.
        /// </summary>
        /// <returns>The total health of all towers.</returns>
        public double GetTowersHealth()
        {
            double towerHealth = 0;
            foreach (Tower tower in _towers)
            {
                towerHealth += tower.Health;
            }
            return towerHealth;
        }

        /// <summary>
        /// Gets the current time in minutes.
        /// </summary>
        /// <returns>The current time in minutes.</returns>
        private double GetCurrentTimeInMinutes()
        {
            DateTime now = DateTime.UtcNow; // Get current time in UTC
            return (now.Hour * 60) + now.Minute + (now.Second / 60.0); // Convert to total minutes
        }

        /// <summary>
        /// Gets the remaining time in minutes.
        /// </summary>
        /// <returns>The remaining time in minutes.</returns>
        public double GetTimeLeftInMinutes()
        {
            double currentTimeInMinutes = GetCurrentTimeInMinutes();
            return _timeLimitInMinutes - (currentTimeInMinutes - _startTime);
        }

        /// <summary>
        /// Checks if the time limit has been reached.
        /// </summary>
        /// <returns>True if the time limit has been reached; otherwise, false.</returns>
        private bool IsTimeLimitReached()
        {
            return GetTimeLeftInMinutes() <= 0;
        }
    }

    /// <summary>
    /// Represents the result of a game update.
    /// </summary>
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

        /// <summary>
        /// Initializes a new instance of the GameResult class.
        /// </summary>
        /// <param name="gameEnded">Indicates whether the game has ended.</param>
        /// <param name="playerScore">The score of the player.</param>
        /// <param name="towerScore">The score of the towers.</param>
        /// <param name="hitResults">The hit results from the towers.</param>
        public GameResult(bool gameEnded, double playerScore, double towerScore, List<ShootingEntityHitResults> hitResults)
        {
            _hitResults = hitResults;
            _gameEnded = gameEnded;
            _playerScore = playerScore;
            _towerScore = towerScore;
        }

        /// <summary>
        /// Gets the score of the player.
        /// </summary>
        public double PlayerScore => _playerScore;

        /// <summary>
        /// Gets the score of the towers.
        /// </summary>
        public double TowerScore => _towerScore;

        /// <summary>
        /// Gets the hit results from the towers.
        /// </summary>
        public List<ShootingEntityHitResults> HitResults => _hitResults;

        /// <summary>
        /// Indicates whether the game has ended.
        /// </summary>
        /// <returns>True if the game has ended; otherwise, false.</returns>
        public bool IsGameEnded()
        {
            return _gameEnded;
        }

        /// <summary>
        /// Gets the leader of the game based on scores.
        /// </summary>
        /// <returns>The leader of the game.</returns>
        public Leader GetLeader()
        {
            Leader result = Leader.tie;
            if (PlayerScore > TowerScore)
            {
                result = Leader.player;
            }
            else if (TowerScore > PlayerScore)
            {
                result = Leader.computer;
            }
            return result;
        }
    }
}
