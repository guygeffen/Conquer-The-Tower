using System;


namespace CttApp
{
    

    public class Shell
    {
        public static double defaultSpeed = 20;
        public static double defaultDamage = 20;
        public static double defaultDamageRadius = 20;

        public double InitialSpeed { get; set; } // Meters per second
        public double Damage { get; set; }
        public double DamageRadius { get; set; } // Meters

        public Shell(double weight, double initialSpeed, double damage, double damageRadius)
        {
            InitialSpeed = initialSpeed;
            Damage = damage;
            DamageRadius = damageRadius;
        }

        public Shell()
        {
            InitialSpeed = defaultSpeed;
            Damage = defaultDamage;
            DamageRadius = defaultDamageRadius;
        }

        public double GetImpactDamage(double distance)
        {
            // Implement logic to calculate damage based on distance.
            // This could involve an inverse square relationship with damage radius.
            return Damage * Math.Pow(DamageRadius / distance, 2);
        }
    }
}