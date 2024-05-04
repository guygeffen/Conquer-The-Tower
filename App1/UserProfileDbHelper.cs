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
            // Check if table exists before creating it

            CreateTableResult t_result=_db.CreateTable<UserProfile>();

            
        }

        public void SaveUserProfile(UserProfile profile)
        {
            // Check if a profile with the same name exists (returns null if none found)
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



        public UserProfile GetUserProfile()
        {
            // Get the first user from the table, assuming only one user is stored
            return _db.Table<UserProfile>().FirstOrDefault();
        }
    }
}
