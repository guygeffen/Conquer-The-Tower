using Android.Gms.Maps.Model;
using System;
using System.Collections.Generic;

namespace CttApp
{
    public class Tower : Entity
    {
        private readonly double _radarRange; // Meters
        private double _weaponTurnSpeed; // Degrees per second
        private double _aimingTime; // Seconds
        private DateTime _lastAimTime; // Time of the last successful aiming
        private int _index;
        private bool _bestAim = false;
        private bool _bestRange = false;

        public int Index { get => _index; }

        public Tower(int index, Location location, double health, Weapon weapon, double radarRange, double weaponTurnSpeed, double aimingTime) : base($"Tower {index}", location, health, weapon)
        {
            _index = index;
            _radarRange = radarRange;
            _weaponTurnSpeed = weaponTurnSpeed;
            _aimingTime = aimingTime;
            _lastAimTime = DateTime.MinValue; // Initialize last aiming time to minimum value
        }

        public ShootingEntityHitResults Attack(Player player)
        {
            Shell shell = new Shell(player.Health, 20);
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

        private bool IsPlayerInRadarRange(Player player)
        {
            return SphericalUtil.ComputeDistanceBetween(
                new LatLng(Location.Latitude, Location.Longitude),
                new LatLng(player.Location.Latitude, player.Location.Longitude)
            ) <= _radarRange;
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

        private double? GetInclinationDifference(Player player, Shell shell)
        {
            double? requiredInclination = Weapon.CalculateRequiredInclination(player.Location);
            return  requiredInclination- Weapon.Inclination;
        }

        private double GetDirectionToPlayer(Player player)
        {
            LatLng from = new LatLng(Location.Latitude, Location.Longitude);
            LatLng to = new LatLng(player.Location.Latitude, player.Location.Longitude);
            return SphericalUtil.ComputeHeading(from, to);
        }

        public override string ToString()
        {
            string description = $"{base.ToString()}<br>" +
                                 $"<b>Radar</b>:{_radarRange}";
            return description;
        }
    }
}
