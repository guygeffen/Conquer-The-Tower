using System;
using SQLite;

namespace CttApp
{
    /// <summary>
    /// Represents a user profile in the application.
    /// </summary>
    public class UserProfile
    {
        public static UserProfile selectedProfile = null;

        /// <summary>
        /// Initializes a new instance of the UserProfile class with specified details.
        /// </summary>
        /// <param name="name">The name of the user.</param>
        /// <param name="age">The age of the user.</param>
        /// <param name="gender">The gender of the user.</param>
        /// <param name="weight">The weight of the user in kilograms.</param>
        /// <param name="height">The height of the user in centimeters.</param>
        public UserProfile(string name, int age, string gender, double weight, int height)
        {
            Name = name;
            Age = age;
            Gender = gender;
            Weight = weight;
            Height = height;
        }

        /// <summary>
        /// Initializes a new instance of the UserProfile class.
        /// </summary>
        public UserProfile()
        {
        }

        /// <summary>
        /// Gets or sets the name of the user. This is the primary key.
        /// </summary>
        [PrimaryKey]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the age of the user.
        /// </summary>
        public int Age { get; set; }

        /// <summary>
        /// Gets or sets the gender of the user.
        /// </summary>
        public string Gender { get; set; }

        /// <summary>
        /// Gets or sets the weight of the user in kilograms.
        /// </summary>
        public double Weight { get; set; }

        /// <summary>
        /// Gets or sets the height of the user in centimeters.
        /// </summary>
        public int Height { get; set; }
    }
}
