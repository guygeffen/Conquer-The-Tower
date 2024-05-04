using System;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Views;
using AndroidX.AppCompat.Widget;
using AndroidX.AppCompat.App;
using Google.Android.Material.FloatingActionButton;
using Google.Android.Material.Snackbar;
using Android.Widget;
using Android.Content;

namespace CttApp
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        private const int UserProfileRequestCode = 1; // Request code for UserProfileActivity
        private const int NewGameRequestCode = 2; // Request code for NewGameActivity
        private const int PlayGameRequestCode = 3; // Request code for UserProfileActivity
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            //Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            FloatingActionButton fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
            fab.Click += FabOnClick;

            // Find the button with ID "userPropButton"
            Button userPropButton = FindViewById<Button>(Resource.Id.userPropButton);

            // Assign a click handler to the button
            userPropButton.Click += UserPropButton_Click;

            // Find the button with ID "userPropButton"
            Button newGameButton = FindViewById<Button>(Resource.Id.newGameButton);

            // Assign a click handler to the button
            newGameButton.Click += NewGameButton_Click;


        }

        private void NewGameButton_Click(object sender, EventArgs e)
        {
            // Create an Intent to launch NewGameActivity
            Intent intent = new Intent(this, typeof(NewGameActivity));

            // Start the activity for result
            StartActivityForResult(intent, NewGameRequestCode);
        }

        private void UserPropButton_Click(object sender, EventArgs e)
        {
            // Create an Intent to launch UserProfileActivity
            Intent intent = new Intent(this, typeof(UserProfileActivity));

            // Start the activity for result
            StartActivityForResult(intent, UserProfileRequestCode);
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (requestCode == UserProfileRequestCode && resultCode == Result.Ok)
            {
                // Handle potential data returned by UserProfileActivity
                // (e.g., display a message or update UI based on saved information)
                string message = data.GetStringExtra("message"); // Example: retrieve a message from UserProfileActivity
                if (message != null)
                {
                    Toast.MakeText(this, message, ToastLength.Short).Show();
                }
            }

            if (requestCode == NewGameRequestCode && resultCode == Result.Ok)
            {
                // Handle potential data returned by NewActivity
                // (e.g., display a message or update UI based on saved information)
                StartActivityForResult(data, PlayGameRequestCode);
            }
        }


        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.action_settings)
            {
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        private void FabOnClick(object sender, EventArgs eventArgs)
        {
            View view = (View) sender;
            Snackbar.Make(view, "Replace with your own action", Snackbar.LengthLong)
                .SetAction("Action", (View.IOnClickListener)null).Show();
        }

        /*public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
           
        }*/

       
	}
}
