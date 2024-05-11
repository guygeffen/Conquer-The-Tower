
namespace CttApp
{
    public class HitResult
    {
        public bool IsHit { get; set; }
        public double Damage { get; set; }
        public Entity HitTarget { get; set; }

        public HitResult(bool isHit, double damage, Entity target)
        {
            IsHit = isHit;
            Damage = damage;
            HitTarget = target;
        }
        public HitResult(bool isHit, double damage) : this(isHit, damage, null)
        {

        }
    }
}