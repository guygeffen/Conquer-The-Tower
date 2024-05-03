using System;
using System.IO;
using SQLite;

namespace CttApp
{
    public class UserProfileDbHelper
    {
        private static string _databasePath;
        private SQLiteConnection _db;

        public UserProfileDbHelper()
        {
            _databasePath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "conquer_the_tower_app.db");
            
            _db = new SQLiteConnection(_databasePath); // Use CreateIfNotExists flag
            //_db.GetTableInfo()
            // Check if table exists before creating it
            if (_db.GetMapping<UserProfile>()==null)
            {
                _db.CreateTable<UserProfile>();
            }
        }

        public void SaveUserProfile(UserProfile profile)
        {
            // Update the profile if it already exists, otherwise insert a new one
            if (_db.Table<UserProfile>().Where(x => x.Name == profile.Name).First()!=null) // Check if a profile with the same name exists
            {
                _db.Update(profile);
            }
            else
            {
                _db.Insert(profile);
            }
        }

        public UserProfile GetUserProfile()
        {
            // Get the first user from the table, assuming only one user is stored
            return _db.Table<UserProfile>().FirstOrDefault();
        }
    }
}
