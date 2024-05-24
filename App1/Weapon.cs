using Android.Gms.Maps.Model;
using System;
using System.Collections.Generic;

namespace CttApp
{
    public class Weapon
    {
        public double Range { get; set; } // Meters
        public double Azimuth { get; set; } // Degrees (0-360) where 0 and 360 is north
        double _inclanation;
        public double Inclination { get { return _inclanation; } set { if (value >= 0 && value <= 90) _inclanation = value; } } // Degrees (0 to 90) where 90 is up
        public Entity Owner { get => owner; set => owner = value; }

        private int roundsShot = 0;
        private int hitsCount = 0;

        private double initialSpeed;
        private const double gravity = 9.81; // Acceleration due to gravity in m/s^2

        private Entity owner;

        public Weapon(double range, double azimuth, double inclination)
        {
            Range = range;
            Azimuth = azimuth;
            Inclination = inclination;
            initialSpeed = Math.Sqrt(Range * gravity);
        }

        public bool IsInRange(Location targetLocation)
        {
            return Owner.Location.DistanceTo(targetLocation) <= Range;
        }

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

        private double DegToRad(double deg)
        {
            return deg * Math.PI / 180;
        }

        public List<HitResult> Fire(List<Entity> targets, Shell shell)
        {
            Location ownerLocation = Owner.Location;
            List<HitResult> hitResults = new List<HitResult>();

            double radInclination = DegToRad(Inclination);
            double radAzimuth = DegToRad(Azimuth);

            double timeOfFlight = (2 * initialSpeed * Math.Sin(radInclination)) / gravity;

            double horizontalDistance = initialSpeed * Math.Cos(radInclination) * timeOfFlight;
            LatLng start = new LatLng(ownerLocation.Latitude, ownerLocation.Longitude);
            LatLng end = SphericalUtil.ComputeOffset(start, horizontalDistance, Azimuth);

            Location projectileLocation = new Location(end.Latitude, end.Longitude, ownerLocation.Altitude);
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

        public override string ToString()
        {
            return $"Range: {Range}, Shot: {roundsShot}, Hit: {hitsCount}";
        }
    }
}
