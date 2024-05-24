using System;

namespace CttApp
{
    public class UserProfile
    {
        public UserProfile(string name, int age, string gender, double weight, int height)
        {
            Name = name;
            Age = age;
            Gender = gender;
            Weight = weight;
            Height = height;
        }
        public UserProfile()
        {
           
        }

        public string Name { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; }
        public double Weight { get; set; }
        public int Height { get; set; }
    }
}