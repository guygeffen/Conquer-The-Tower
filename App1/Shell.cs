using System;


namespace CttApp
{
    

    public class Shell
    {
        
        public static double defaultDamage = 20;
        public static double defaultDamageRadius = 20;

        //public double InitialSpeed { get; set; } // Meters per second
        public double Damage { get; set; }
        public double DamageRadius { get; set; } // Meters

        public Shell( double damage, double damageRadius)
        {
            Damage = damage;
            DamageRadius = damageRadius;
        }

        public static double CalculateInitialSpeed(double range)
        {
            const double gravity = 9.81; // Acceleration due to gravity in m/s²
            return Math.Sqrt(range * gravity);
        }

        public Shell()
        {
            //InitialSpeed = defaultSpeed;
            Damage = defaultDamage;
            DamageRadius = defaultDamageRadius;
        }

        public double GetImpactDamage(double distance)
        {
            // Implement logic to calculate damage based on distance.
            // This could involve an inverse square relationship with damage radius.
            //return Damage * Math.Pow(DamageRadius / distance, 2);
            if (distance< 5)
            {
                return Damage;
            }
            else if (distance> DamageRadius)
            {
                return 0;
            }
            return Damage * (1 - (distance /DamageRadius));
        }

    }
}