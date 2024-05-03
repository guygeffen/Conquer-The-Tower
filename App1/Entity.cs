
namespace CttApp
{
    public class Entity
    {
        public Location Location { get; set; }
        public double Health { get; set; }
        public Weapon Weapon { get; set; }

        public Entity(Location location, double health, Weapon weapon)
        {
            Location = location;
            Health = health;
            Weapon = weapon;
        }

        public virtual void TakeDamage(double damage)
        {
            Health -= damage;
        }

        public bool IsDestroyed()
        {
            return Health <= 0;
        }
    }
}