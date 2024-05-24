
using System.Collections.Generic;

namespace CttApp
{
    public class HitResult
    {
        public bool IsHit { get; private set; }
        public double Damage { get; private set; }
        public Entity HitTarget { get; private set; }
        public Location HitLocation { get; private set; }


        //public Shell Projectile { get => Projectile; set => Projectile = value; }

        public double HitRadius { get; set; }

 

        public HitResult(bool isHit, double damage,Location hitLocation, Entity target, double hitRadius)
        {
            HitRadius = hitRadius;
            IsHit = isHit;
            Damage = damage;
            HitTarget = target;
            HitLocation = hitLocation;
           
            //Projectile = shell;
        }
  
    }

    public class ShootingEntityHitResults

    {
        private readonly List<HitResult> hitResults;
        private readonly Entity entity;

        public ShootingEntityHitResults(List<HitResult> hitResults, Entity entity)
        {
            this.hitResults = hitResults;
            this.entity = entity;
        }

        public List<HitResult> HitResults { get => hitResults;}
        public Entity Entity { get => entity; }
    }
}