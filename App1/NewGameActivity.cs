using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using AndroidX.AppCompat.App;
using System;

namespace CttApp
{
    /// <summary>
    /// Activity for setting up a new game.
    /// </summary>
    [Activity(Label = "New Game")]
    public class NewGameActivity : AppCompatActivity
    {
        private EditText NumTowersInput;
        private EditText gameRadiusInput;
        private EditText gameTimeInput;
        private EditText RangeInput;
        private TextView currentUserProfile;
        private Button startGameButton;
        private Button cancelButton;
        public const string num_towers_key = "NumTowers";
        public const string game_radius_key = "GameRadius";
        public const string game_time_key = "GameTime";
        public const string range_key = "Range";
        private UserProfileDbHelper _dbHelper;

        /// <summary>
        /// Called when the activity is first created.
        /// </summary>
        /// <param name="savedInstanceState">The saved instance state.</param>
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our custom layout for this Activity
            SetContentView(Resource.Layout.activity_new_game);

            // Initialize database helper
            _dbHelper = UserProfileDbHelper.GetInstance();

            // Find UI elements from the layout
            NumTowersInput = FindViewById<EditText>(Resource.Id.numTowersInput);
            gameRadiusInput = FindViewById<EditText>(Resource.Id.gameRadiusInput);
            gameTimeInput = FindViewById<EditText>(Resource.Id.gameTimeInput);
            RangeInput = FindViewById<EditText>(Resource.Id.rangeinput);
            currentUserProfile = FindViewById<TextView>(Resource.Id.currentUserProfile);
            startGameButton = FindViewById<Button>(Resource.Id.startGameButton);
            cancelButton = FindViewById<Button>(Resource.Id.cancelButton);

            // Start game button click handler
            startGameButton.Click += OnStartGameButtonClicked;
            cancelButton.Click += OnCancelButtonClicked;

            // Load and display the current user profile
            LoadCurrentUserProfile();
        }

        /// <summary>
        /// Loads the current user profile and displays it.
        /// </summary>
        private void LoadCurrentUserProfile()
        {
            var userProfile = _dbHelper.GetUserProfile();
            if (userProfile != null)
            {
                currentUserProfile.Text = $"Current User: {userProfile.Name}";
            }
            else
            {
                ShowNoUserAlert();
            }
        }

        /// <summary>
        /// Shows an alert dialog if no user profile is found.
        /// </summary>
        private void ShowNoUserAlert()
        {
            AndroidX.AppCompat.App.AlertDialog.Builder alert = new AndroidX.AppCompat.App.AlertDialog.Builder(this);
            alert.SetTitle("No User Found");
            alert.SetMessage("No user profile found. Please create a new user profile.");
            alert.SetPositiveButton("OK", (sender, args) =>
            {
                // Redirect to UserProfileActivity to create a new user profile
                Intent intent = new Intent(this, typeof(UserProfileActivity));
                StartActivity(intent);
                Finish();
            });
            alert.Show();
        }

        /// <summary>
        /// Handles the cancel button click event.
        /// </summary>
        private void OnCancelButtonClicked(object sender, EventArgs e)
        {
            Intent intent = new Intent();
            SetResult(Result.Canceled, intent); // Set result code and data (optional)
            Finish(); // Close this activity
        }

        /// <summary>
        /// Handles the start game button click event.
        /// </summary>
        private void OnStartGameButtonClicked(object sender, EventArgs e)
        {
            // Validate and parse user input
            if (int.TryParse(NumTowersInput.Text, out int numTowers) &&
                int.TryParse(gameRadiusInput.Text, out int gameRadius) &&
                int.TryParse(gameTimeInput.Text, out int gameTime) &&
                int.TryParse(RangeInput.Text, out int rangeInput))
            {
                Intent intent = new Intent(this, typeof(PlayGameActivity));
                intent.PutExtra(num_towers_key, numTowers);
                intent.PutExtra(game_radius_key, gameRadius);
                intent.PutExtra(game_time_key, gameTime);
                intent.PutExtra(range_key, rangeInput);
                SetResult(Result.Ok, intent); // Set result code and data (optional)
                Finish(); // Close this activity
            }
            else
            {
                // Handle invalid input (show toast or message)
                Toast.MakeText(this, "Invalid input. Please enter valid values.", ToastLength.Short).Show();
            }
        }
    }
}
