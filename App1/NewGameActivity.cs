using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CttApp
{
    using System;
    using Android.App;
    using Android.OS;
    using Android.Views;
    using AndroidX.AppCompat.App;
    using Android.Widget;
    using Android.Content;

    namespace YourProjectName.Droid
    {
        [Activity(Label = "New Game")]
        public class NewGameActivity : AppCompatActivity
        {
            private EditText numTowersInput;
            private EditText gameRadiusInput;
            private EditText gameTimeInput;
            private Button startGameButton;

            protected override void OnCreate(Bundle savedInstanceState)
            {
                base.OnCreate(savedInstanceState);

                // Set our custom layout for this Activity
                SetContentView(Resource.Layout.activity_new_game);

                // Find UI elements from the layout
                numTowersInput = FindViewById<EditText>(Resource.Id.numTowersInput);
                gameRadiusInput = FindViewById<EditText>(Resource.Id.gameRadiusInput);
                gameTimeInput = FindViewById<EditText>(Resource.Id.gameTimeInput);
                startGameButton = FindViewById<Button>(Resource.Id.startGameButton);

                // Start game button click handler
                startGameButton.Click += OnStartGameButtonClicked;
            }

            private void OnStartGameButtonClicked(object sender, EventArgs e)
            {
                int numTowers;
                float gameRadius;
                TimeSpan gameTime;

                // Validate and parse user input
                if (int.TryParse(numTowersInput.Text, out numTowers) &&
                    float.TryParse(gameRadiusInput.Text, out gameRadius) &&
                    TimeSpan.TryParse(gameTimeInput.Text, out gameTime))
                {
                    // Start the game activity with the new game parameters
                    /*
                    var intent = new Intent(this, typeof(PlayGameActivity));
                    intent.PutExtra("NumTowers", numTowers);
                    intent.PutExtra("GameRadius", gameRadius);
                    intent.PutExtra("GameTime", gameTime.TotalMilliseconds);
                    StartActivity(intent);*/
                }
                else
                {
                    // Handle invalid input (show toast or message)
                    Toast.MakeText(this, "Invalid input. Please enter valid values.", ToastLength.Short);
                }
            }
        }
    }

}