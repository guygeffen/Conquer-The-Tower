
using System;
using System.Collections.Generic;

namespace CttApp
{
    public class Tower : Entity
    {
        
        private double _radarRange; // Meters
        private double _weaponTurnSpeed; // Degrees per second
        private double _aimingTime; // Seconds
        private DateTime _lastAimTime; // Time of the last successful aiming

        public Tower(Location location, double health, Weapon weapon, double radarRange, double weaponTurnSpeed, double aimingTime) : base(location, health, weapon)
        {
            
           
            _radarRange = radarRange;
            _weaponTurnSpeed = weaponTurnSpeed;
            _aimingTime = aimingTime;
            _lastAimTime = DateTime.MinValue; // Initialize last aiming time to minimum value
        }

        public void Attack(Player player)
        {
            if (!IsPlayerInRadarRange(player))
            {
                return; // Player not detected
            }

            // Aim the weapon towards the player (consider aiming time)
            double timeSinceLastAim = GetTimeSinceLastAim();

            if (timeSinceLastAim >= _aimingTime)
            {
                // Player fully aimed at
                double angleDifference = GetWeaponAngleDifference(player);
                if (Math.Abs(angleDifference) > 2) // Minimum turn amount is 2 degrees
                {
                    // Gradually adjust weapon direction towards player
                    Weapon.Azimuth += Math.Sign(angleDifference) * Math.Min(Math.Abs(angleDifference), _weaponTurnSpeed); // Limit turn amount
                }
                else
                {

                    List<Entity> targets = new List<Entity>();
                    targets.Add(player);
                    // Weapon aimed, fire at player
                    Weapon.Fire(targets, Location, new Shell());
                    _lastAimTime = DateTime.UtcNow; // Update last aiming time
                }
            }
        }

        private bool IsPlayerInRadarRange(Player player)
        {
            return player.Location.DistanceTo(Location) <= _radarRange;
        }

        private double GetTimeSinceLastAim()
        {
            TimeSpan timeSpan = DateTime.UtcNow - _lastAimTime;
            return timeSpan.TotalSeconds;
        }

        private double GetWeaponAngleDifference(Player player)
        {
            double playerAngle = GetDirectionToPlayer(player);
            return playerAngle - Weapon.Azimuth;
        }

        private double GetDirectionToPlayer(Player player)
        {
            double playerLatitudeRad = player.Location.Latitude * (Math.PI / 180); // Convert latitude to radians
            double playerLongitudeRad = player.Location.Longitude * (Math.PI / 180); // Convert longitude to radians
            double towerLatitudeRad = Location.Latitude * (Math.PI / 180);
            double towerLongitudeRad = Location.Longitude * (Math.PI / 180);

            double dlon = towerLongitudeRad - playerLongitudeRad;

            // Use Haversine formula to calculate the central angle between player and tower (in radians)
            double centralAngle = 2 * Math.Asin(Math.Min(1, Math.Sqrt(Math.Pow(Math.Sin(dlon / 2), 2) +
                                                          Math.Cos(towerLatitudeRad) * Math.Cos(playerLatitudeRad) * Math.Pow(Math.Sin((playerLongitudeRad - towerLongitudeRad) / 2), 2))));

            // Calculate bearing using law of cosines (considering Earth's radius)
            //double earthRadius = 6371000; // Meters (approximate Earth's radius)
            double bearing = Math.Atan2(Math.Sin(dlon) * Math.Cos(playerLatitudeRad),
                                        Math.Cos(towerLatitudeRad) * Math.Sin(playerLatitudeRad) - Math.Sin(towerLatitudeRad) * Math.Cos(playerLatitudeRad) * Math.Cos(dlon));

            // Convert bearing to degrees between 0 and 360
            bearing = Math.Floor(Utils.Deg2Rad(bearing + Math.PI)) % (2 * Math.PI);
            bearing = Utils.Deg2Rad(bearing); // Convert back to degrees using custom function


            // Adjust for negative bearings (since arctangent result is in range -pi/2 to pi/2)
            if (bearing < 0)
            {
                bearing += 2 * Math.PI;
            }

            return bearing * (180 / Math.PI); // Convert to degrees
        }
    }


}