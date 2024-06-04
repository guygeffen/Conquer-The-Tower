using System;
using System.Collections.Generic;
using System.IO;
using SQLite;

namespace CttApp
{
    /// <summary>
    /// Helper class for managing user profiles in the database.
    /// </summary>
    public class UserProfileDbHelper
    {
        private static string _databasePath;
        private readonly SQLiteConnection _db;
        private static UserProfileDbHelper instance;

        /// <summary>
        /// Private constructor to initialize the database connection and create tables if they do not exist.
        /// </summary>
        private UserProfileDbHelper()
        {
            _databasePath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "conquer_the_tower_app.db");
            _db = new SQLiteConnection(_databasePath);
            _db.CreateTable<UserProfile>();
        }

        /// <summary>
        /// Gets the singleton instance of the UserProfileDbHelper.
        /// </summary>
        /// <returns>The singleton instance of UserProfileDbHelper.</returns>
        public static UserProfileDbHelper GetInstance()
        {
            if (instance == null)
            {
                instance = new UserProfileDbHelper();
            }
            return instance;
        }

        /// <summary>
        /// Saves the user profile to the database. If a profile with the same name exists, it updates it; otherwise, it inserts a new profile.
        /// </summary>
        /// <param name="profile">The user profile to save.</param>
        public void SaveUserProfile(UserProfile profile)
        {
            var existingProfile = _db.Table<UserProfile>().FirstOrDefault(x => x.Name == profile.Name);

            if (existingProfile != null)
            {
                _db.Update(profile);
            }
            else
            {
                _db.Insert(profile);
            }
        }

        /// <summary>
        /// Gets the user profile from the database. Assumes only one user is stored.
        /// </summary>
        /// <returns>The user profile.</returns>
        public UserProfile GetUserProfile()
        {
            if (UserProfile.selectedProfile == null)
            {
                UserProfile.selectedProfile = _db.Table<UserProfile>().FirstOrDefault();
            }
            return UserProfile.selectedProfile;
        }

        /// <summary>
        /// Gets all user profiles from the database.
        /// </summary>
        /// <returns>A list of all user profiles.</returns>
        public List<UserProfile> GetAllUserProfiles()
        {
            return _db.Table<UserProfile>().ToList();
        }
    }
}
