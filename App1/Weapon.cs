

using System;
using System.Collections.Generic;

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


        public List<HitResult> Fire( List<Entity> targets, Location ownerLocation, Shell shell)
        {
            List<HitResult> hitResults = new List<HitResult>();
            double gravity = 9.81; // Acceleration due to gravity in m/s^2
            double timeIncrement = 0.1; // Time step for simulation in seconds

            // Initial velocity Azimuth
            double velocityX = shell.InitialSpeed * Math.Cos(Inclination * Math.PI / 180) * Math.Sin(Azimuth * Math.PI / 180);
            double velocityY = shell.InitialSpeed * Math.Sin(Inclination * Math.PI / 180);
            double velocityZ = shell.InitialSpeed * Math.Cos(Inclination * Math.PI / 180) * Math.Cos(Azimuth * Math.PI / 180);

            // Simulate the flight of the shell
            double time = 0;
            Location projectileLocation = new Location(ownerLocation.Latitude, ownerLocation.Longitude, ownerLocation.Altitude);
            bool hitDetected = false;

            while (projectileLocation.Altitude > 0 && !hitDetected)
            {
                time += timeIncrement;
                projectileLocation.Latitude += velocityX * timeIncrement;
                projectileLocation.Longitude += velocityZ * timeIncrement;
                projectileLocation.Altitude += velocityY * timeIncrement - 0.5 * gravity * time * time; // Updating altitude with vertical displacement due to gravity

                foreach (var target in targets)
                {
                    double distanceToTarget = projectileLocation.DistanceTo(target.Location);
                    if (distanceToTarget <= shell.DamageRadius)
                    {
                        double damage = shell.GetImpactDamage(distanceToTarget);
                        target.TakeDamage(damage);
                        hitResults.Add(new HitResult(true, damage, target));
                        hitDetected = true;
                    }
                }
            }

            if (!hitDetected)
            {
                hitResults.Add(new HitResult(false, 0));
            }

            return hitResults;
        }
    }
  
}