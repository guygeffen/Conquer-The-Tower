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
using AndroidX.AppCompat.App;


    namespace CttApp
    {
        [Activity(Label = "New Game")]
        public class NewGameActivity : AppCompatActivity
        {
            private EditText numTowersInput;
            private EditText gameRadiusInput;
            private EditText gameTimeInput;
            private Button startGameButton;
            private Button cancelButton;

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
                cancelButton = FindViewById<Button>(Resource.Id.cancelButton);
            

                // Start game button click handler
                startGameButton.Click += OnStartGameButtonClicked;
                cancelButton.Click += OnCancelButtonClicked;

            }

            private void OnCancelButtonClicked(object sender, EventArgs e)
            {
                Intent intent = new Intent();
                SetResult(Result.Canceled, intent); // Set result code and data (optional)
                Finish(); // Close this activity
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
                    Intent intent = new Intent(this, typeof(PlayGameActivity));
                    intent.PutExtra("NumTowers", numTowers);
                    intent.PutExtra("GameRadius", gameRadius);
                    intent.PutExtra("GameTime", gameTime.TotalMilliseconds);
                    SetResult(Result.Ok, intent); // Set result code and data (optional)
                    Finish(); // Close this activity
                }
                else
                {
                    // Handle invalid input (show toast or message)
                    Toast.MakeText(this, "Invalid input. Please enter valid values.", ToastLength.Short);
                }
            }
        }
    }
