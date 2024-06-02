using Android.Gms.Maps.Model;
using System;
using System.Collections.Generic;

namespace CttApp
{
    /// <summary>
    /// Represents a weapon with specific range, azimuth, inclination, and rate of fire.
    /// </summary>
    public class Weapon
    {
        /// <summary>
        /// Gets or sets the range of the weapon in meters.
        /// </summary>
        public double Range { get; set; } // Meters

        /// <summary>
        /// Gets or sets the azimuth of the weapon in degrees (0-360) where 0 and 360 is north.
        /// </summary>
        public double Azimuth { get; set; } // Degrees (0-360) where 0 and 360 is north

        private double _inclination;

        /// <summary>
        /// Gets or sets the inclination of the weapon in degrees (0 to 90) where 90 is up.
        /// </summary>
        public double Inclination
        {
            get { return _inclination; }
            set { if (value >= 0 && value <= 90) _inclination = value; }
        } // Degrees (0 to 90) where 90 is up

        /// <summary>
        /// Gets or sets the owner of the weapon.
        /// </summary>
        public Entity Owner { get => owner; set => owner = value; }

        private int roundsShot = 0;
        private int hitsCount = 0;
        private readonly double initialSpeed;
        private const double gravity = 9.81; // Acceleration due to gravity in m/s^2
        private Entity owner;
        private int shellsPerMinute;
        private DateTime lastFireTime;

        /// <summary>
        /// Gets or sets the rate of fire in shells per minute.
        /// </summary>
        public int ShellsPerMinute
        {
            get { return shellsPerMinute; }
            set
            {
                if (value > 0)
                    shellsPerMinute = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the Weapon class with specified parameters.
        /// </summary>
        /// <param name="range">The range of the weapon in meters.</param>
        /// <param name="azimuth">The azimuth of the weapon in degrees (0-360).</param>
        /// <param name="inclination">The inclination of the weapon in degrees (0 to 90).</param>
        /// <param name="shellsPerMinute">The rate of fire in shells per minute.</param>
        public Weapon(double range, double azimuth, double inclination, int shellsPerMinute)
        {
            Range = range;
            Azimuth = azimuth;
            Inclination = inclination;
            initialSpeed = Math.Sqrt(Range * gravity);
            ShellsPerMinute = shellsPerMinute;
            lastFireTime = DateTime.MinValue;
        }

        /// <summary>
        /// Calculates the required inclination to hit a target at the specified location.
        /// </summary>
        /// <param name="targetLocation">The location of the target.</param>
        /// <returns>The required inclination in degrees, or null if the target is out of range.</returns>
        public double? CalculateRequiredInclination(Location targetLocation)
        {
            Location ownerLocation = Owner.Location;
            double R = SphericalUtil.ComputeDistanceBetween(
                new LatLng(ownerLocation.Latitude, ownerLocation.Longitude),
                new LatLng(targetLocation.Latitude, targetLocation.Longitude)
            );
            double v0 = initialSpeed;

            double valueInsideAsin = (R * gravity) / (v0 * v0);

            if (valueInsideAsin < -1 || valueInsideAsin > 1)
            {
                return null;
            }
            double angleRadians = 0.5 * Math.Asin(valueInsideAsin);
            double angleDegrees = angleRadians * (180.0 / Math.PI);
            return angleDegrees;
        }

        /// <summary>
        /// Converts degrees to radians.
        /// </summary>
        /// <param name="deg">The angle in degrees.</param>
        /// <returns>The angle in radians.</returns>
        private double DegToRad(double deg)
        {
            return deg * Math.PI / 180;
        }

        /// <summary>
        /// Fires the weapon towards the targets and calculates hit results.
        /// </summary>
        /// <param name="targets">A list of potential target entities.</param>
        /// <param name="shell">The shell to be fired.</param>
        /// <returns>A list of hit results.</returns>
        public List<HitResult> Fire(List<Entity> targets, Shell shell)
        {
            // Enforce rate of fire
            if (!CanShoot())
            {
                return new List<HitResult>(); // No firing if not enough time has passed
            }
            lastFireTime = DateTime.UtcNow;

            Location ownerLocation = Owner.Location;
            List<HitResult> hitResults = new List<HitResult>();

            double radInclination = DegToRad(Inclination);
            double radAzimuth = DegToRad(Azimuth);

            double timeOfFlight = (2 * initialSpeed * Math.Sin(radInclination)) / gravity;

            double horizontalDistance = initialSpeed * Math.Cos(radInclination) * timeOfFlight;
            LatLng start = new LatLng(ownerLocation.Latitude, ownerLocation.Longitude);
            LatLng end = SphericalUtil.ComputeOffset(start, horizontalDistance, Azimuth);

            Location projectileLocation = new Location(end.Latitude, end.Longitude);
            bool hitDetected = false;

            foreach (var target in targets)
            {
                double distanceToTarget = SphericalUtil.ComputeDistanceBetween(
                    new LatLng(projectileLocation.Latitude, projectileLocation.Longitude),
                    new LatLng(target.Location.Latitude, target.Location.Longitude)
                );

                if (distanceToTarget <= shell.DamageRadius)
                {
                    double damage = shell.GetImpactDamage(distanceToTarget);
                    target.TakeDamage(damage);
                    hitResults.Add(new HitResult(true, damage, projectileLocation, target, shell.DamageRadius));
                    hitDetected = true;
                    hitsCount++;
                }
            }

            if (!hitDetected)
            {
                hitResults.Add(new HitResult(false, 0, projectileLocation, null, shell.DamageRadius));
            }
            roundsShot++;

            return hitResults;
        }

        /// <summary>
        /// Determines if the weapon can shoot based on the rate of fire and the owner's status.
        /// </summary>
        /// <returns>True if the weapon can shoot, otherwise false.</returns>
        public bool CanShoot()
        {
            TimeSpan timeSinceLastFire = DateTime.UtcNow - lastFireTime;
            double minTimeBetweenShots = 60.0 / ShellsPerMinute; // Convert shells per minute to seconds per shot
            return (timeSinceLastFire.TotalSeconds >= minTimeBetweenShots) && !owner.IsDead();
        }

        /// <summary>
        /// Returns a string representation of the weapon's stats.
        /// </summary>
        /// <returns>A string containing the weapon's range, shots fired, and hits.</returns>
        public override string ToString()
        {
            return $"<b>Range:</b> {(int)Range}, <b>Shot:</b> {roundsShot}, <b>Hit:</b> {hitsCount}";
        }
    }
}
