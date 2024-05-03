

using System;

namespace CttApp
{
    public class Weapon
    {
        public double Range { get; set; } // Meters
        public double Azimuth { get; set; } // Degrees (0-360)
        public double Inclination { get; set; } // Degrees (-90 to 90)

        public Weapon(double range, double azimuth, double inclination)
        {
            Range = range;
            Azimuth = azimuth;
            Inclination = inclination;
        }

        public bool IsInRange(Location targetLocation, Location ownerLocation)
        {
            return ownerLocation.DistanceTo(targetLocation) <= Range;
        }

        public HitResult Fire(Entity target, Location ownerLocation, Shell shell)
        {
            if (!IsInRange(target.Location, ownerLocation))
            {
                return new HitResult(false, 0); // Out of range, no hit
            }

            // Calculate initial velocity components based on weapon aim and shell speed
            double initialSpeed = shell.InitialSpeed; // Meters per second
            double velocityX = initialSpeed * Math.Cos(Inclination * (Math.PI / 180)) * Math.Cos(Azimuth * (Math.PI / 180));
            double velocityY = initialSpeed * Math.Sin(Inclination * (Math.PI / 180));
            double velocityZ = initialSpeed * Math.Sin(Azimuth * (Math.PI / 180));

            // Simulate ballistic trajectory using shell properties and initial velocity (replace with your implementation)
            double distance = target.Location.DistanceTo(ownerLocation);
            double flightTime = distance / Math.Sqrt(Math.Pow(velocityX, 2) + Math.Pow(velocityY, 2)); // Flight time considering x and y components

            // Consider gravity (replace with more precise physics calculations)
            double gravity = 9.81; // Meters per second squared
            double verticalDisplacement = 0.5 * gravity * Math.Pow(flightTime, 2); // Simplified vertical displacement

            // Calculate target position based on initial velocity and flight time
            double targetX = ownerLocation.Latitude + velocityX * flightTime;
            double targetY = ownerLocation.Longitude + velocityY * flightTime;

            // Check if target position is within a hit radius
            double hitRadius = shell.DamageRadius; // Meters
            double distanceToTarget = Math.Sqrt(Math.Pow(ownerLocation.Latitude - targetX, 2) + Math.Pow(ownerLocation.Longitude - targetY, 2));
            bool isHit = distanceToTarget <= hitRadius;

            if (isHit)
            {
                target.TakeDamage(shell.GetImpactDamage(distanceToTarget)); // Damage based on distance
            }

            return new HitResult(isHit, isHit ? shell.GetImpactDamage(distanceToTarget) : 0);
        }
    }

}