using System;

namespace CttApp
{
    public class UserProfile
    {
        public UserProfile(string name, DateTime dateOfBirth, string gender, double weight)
        {
            Name = name;
            DateOfBirth = dateOfBirth;
            Gender = gender;
            Weight = weight;
        }
        public UserProfile()
        {
           
        }

        public string Name { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public double Weight { get; set; }
    }
}