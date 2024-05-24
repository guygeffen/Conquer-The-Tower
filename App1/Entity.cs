
using System;

namespace CttApp
{
    public class Entity
    {
        private Location _location;
        public Location Location { 
            get 
            { return _location; }  
            set { 
              
                 LocationChanged(_location, value);
                _location = value;         
            } 
        }

        public double Health { get; private set; }
        public Weapon Weapon { get; private set; }

        public string Name { get; private set; }

        

        protected virtual void LocationChanged (Location oldLocation, Location newLocation)
        {
            // do nothing
        }
       

        public Entity(String name, Location location, double health, Weapon weapon)
        {
            Name = name;
            Location = location;
            Health = health;
            Weapon = weapon;
            weapon.Owner = this;
        }

        public virtual void TakeDamage(double damage)
        {
            Health -= damage;
            if(Health<0)
            {
                Health = 0;
            }
        }

        public bool IsDestroyed()
        {
            return Health <= 0;
        }


        public override string ToString()
        {
            return $"<b>Health</b> {Health}<br>" +
                   $"<b>Weapon</b> {Weapon}";
        }
    }
}