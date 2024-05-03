
namespace CttApp
{
    public class HitResult
    {
        public bool IsHit { get; private set; }
        public double Damage { get; private set; }

        public HitResult(bool isHit, double damage)
        {
            IsHit = isHit;
            Damage = damage;
        }
    }
}