using Android.Gms.Maps.Model;
using System;
using System.Collections.Generic;

namespace CttApp
{
    /// <summary>
    /// Represents a defensive tower entity with radar and weapon capabilities.
    /// </summary>
    public class Tower : Entity
    {
        private readonly double _radarRange; // Meters
        private readonly double _weaponTurnSpeed; // Degrees per second
        private readonly double _aimingTime; // Seconds
        private DateTime _lastAimTime; // Time of the last successful aiming
        private readonly int _index;
        private bool _bestAim = false;
        private bool _bestRange = false;

        /// <summary>
        /// Gets the index of the tower.
        /// </summary>
        public int Index { get => _index; }

        /// <summary>
        /// Initializes a new instance of the Tower class with specified parameters.
        /// </summary>
        /// <param name="index">The index of the tower.</param>
        /// <param name="location">The location of the tower.</param>
        /// <param name="health">The health of the tower.</param>
        /// <param name="weapon">The weapon equipped on the tower.</param>
        /// <param name="radarRange">The radar range of the tower in meters.</param>
        /// <param name="weaponTurnSpeed">The turn speed of the weapon in degrees per second.</param>
        /// <param name="aimingTime">The aiming time required before firing in seconds.</param>
        public Tower(int index, Location location, double health, Weapon weapon, double radarRange, double weaponTurnSpeed, double aimingTime)
            : base($"Tower {index}", location, health, weapon)
        {
            _index = index;
            _radarRange = radarRange;
            _weaponTurnSpeed = weaponTurnSpeed;
            _aimingTime = aimingTime;
            _lastAimTime = DateTime.MinValue; // Initialize last aiming time to minimum value
        }

        /// <summary>
        /// Attacks the specified player if they are within radar range.
        /// </summary>
        /// <param name="player">The player to be attacked.</param>
        /// <returns>The results of the attack.</returns>
        public ShootingEntityHitResults Attack(Player player)
        {
            Shell shell = new Shell(GameConstants.TowerShellDamage, GameConstants.TowerShellDamageRadius);
            if (IsPlayerInRadarRange(player))
            {
                double timeSinceLastAim = GetTimeSinceLastAim();

                if (timeSinceLastAim >= _aimingTime)
                {
                    List<Entity> targets = new List<Entity>() { player };

                    double angleDifference = GetWeaponAngleDifference(player);
                    double? inclinationDifference = GetInclinationDifference(player, shell);

                    if (Math.Abs(angleDifference) > _weaponTurnSpeed)
                    {
                        Weapon.Azimuth += Math.Sign(angleDifference) * Math.Min(Math.Abs(angleDifference), _weaponTurnSpeed);
                        _bestAim = false;
                    }
                    else
                    {
                        _bestAim = true;
                    }

                    if (_bestAim)
                    {
                        if (!inclinationDifference.HasValue)
                        {
                            _bestRange = false;
                        }
                        else if (Math.Abs(inclinationDifference.Value) > _weaponTurnSpeed)
                        {
                            Weapon.Inclination += Math.Sign(inclinationDifference.Value) * Math.Min(Math.Abs(inclinationDifference.Value), _weaponTurnSpeed);
                            _bestRange = false;
                        }
                        else
                        {
                            _bestRange = true;
                        }
                    }

                    if (_bestAim && _bestRange)
                    {
                        List<HitResult> hitResults = Weapon.Fire(targets, shell);
                        _bestAim = false;
                        _bestRange = false;
                        return new ShootingEntityHitResults(hitResults, this);
                    }

                    _lastAimTime = DateTime.UtcNow; // Update last aiming time
                }
            }

            List<HitResult> hitresults = new List<HitResult>() { new HitResult(false, 0, null, player, 0) };
            return new ShootingEntityHitResults(hitresults, this); // Player not detected
        }

        /// <summary>
        /// Checks if the player is within the radar range of the tower.
        /// </summary>
        /// <param name="player">The player to be checked.</param>
        /// <returns>True if the player is within radar range, otherwise false.</returns>
        private bool IsPlayerInRadarRange(Player player)
        {
            return SphericalUtil.ComputeDistanceBetween(Location.GetLatLng(),
                                         player.Location.GetLatLng() )<= _radarRange;
        }

        /// <summary>
        /// Gets the time since the last successful aiming in seconds.
        /// </summary>
        /// <returns>The time since the last successful aiming in seconds.</returns>
        private double GetTimeSinceLastAim()
        {
            TimeSpan timeSpan = DateTime.UtcNow - _lastAimTime;
            return timeSpan.TotalSeconds;
        }

        /// <summary>
        /// Gets the difference in weapon azimuth angle required to aim at the player.
        /// </summary>
        /// <param name="player">The player to aim at.</param>
        /// <returns>The difference in weapon azimuth angle.</returns>
        private double GetWeaponAngleDifference(Player player)
        {
            double playerAngle = GetDirectionToPlayer(player);
            return playerAngle - Weapon.Azimuth;
        }

        /// <summary>
        /// Gets the difference in weapon inclination required to hit the player.
        /// </summary>
        /// <param name="player">The player to hit.</param>
        /// <param name="shell">The shell to be fired.</param>
        /// <returns>The difference in weapon inclination.</returns>
        private double? GetInclinationDifference(Player player, Shell shell)
        {
            double? requiredInclination = Weapon.CalculateRequiredInclination(player.Location);
            return requiredInclination - Weapon.Inclination;
        }

        /// <summary>
        /// Gets the direction to the player in degrees from the tower's location.
        /// </summary>
        /// <param name="player">The player to find the direction to.</param>
        /// <returns>The direction to the player in degrees.</returns>
        private double GetDirectionToPlayer(Player player)
        {
            LatLng from = new LatLng(Location.Latitude, Location.Longitude);
            LatLng to = new LatLng(player.Location.Latitude, player.Location.Longitude);
            return SphericalUtil.ComputeHeading(from, to);
        }

        /// <summary>
        /// Returns a string representation of the tower's stats.
        /// </summary>
        /// <returns>A string containing the tower's radar range and base entity description.</returns>
        public override string ToString()
        {
            string description = $"{base.ToString()}<br>" +
                                 $"<b>Radar</b>:{_radarRange}";
            return description;
        }
    }
}
