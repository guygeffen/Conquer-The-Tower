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
using AndroidX.Core.Content;
using Android;
using Android.Content.PM;
using AndroidX.Core.App;

namespace CttApp
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        private const int UserProfileRequestCode = 1; // Request code for UserProfileActivity
        private const int NewGameRequestCode = 2; // Request code for NewGameActivity
        private const int PlayGameRequestCode = 3; // Request code for UserProfileActivity
        private const int MY_PERMISSIONS_REQUEST_LOCATION = 1000;
       

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            //Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);


            // Find the button with ID "userPropButton"
            Button userPropButton = FindViewById<Button>(Resource.Id.userPropButton);

            // Assign a click handler to the button
            userPropButton.Click += UserPropButton_Click;

            // Find the button with ID "userPropButton"
            Button newGameButton = FindViewById<Button>(Resource.Id.newGameButton);

            // Assign a click handler to the button
            newGameButton.Click += NewGameButton_Click;

            //check location permission
            if (!CheckLocationPermissions())
            {
                RequestLocationPermissions();
            }

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

       
        private bool CheckLocationPermissions()
        {
            return ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessFineLocation) == (int)Permission.Granted &&
                   ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessCoarseLocation) == (int)Permission.Granted;
        }

        private void RequestLocationPermissions()
        {
            if (ActivityCompat.ShouldShowRequestPermissionRationale(this, Manifest.Permission.AccessFineLocation))
            {
                // Show an explanation to the user asynchronously
                AndroidX.AppCompat.App.AlertDialog.Builder alert = new AndroidX.AppCompat.App.AlertDialog.Builder(this);
                alert.SetTitle("Location Permission Needed");
                alert.SetMessage("This app needs location permissions to function properly.");
                alert.SetPositiveButton("OK", (senderAlert, args) =>
                {
                    ActivityCompat.RequestPermissions(this,
                        new String[] { Manifest.Permission.AccessFineLocation, Manifest.Permission.AccessCoarseLocation },
                        MY_PERMISSIONS_REQUEST_LOCATION);
                });
                alert.Show();
            }
            else
            {
                // No explanation needed, we can request the permission.
                ActivityCompat.RequestPermissions(this,
                    new String[] { Manifest.Permission.AccessFineLocation, Manifest.Permission.AccessCoarseLocation },
                    MY_PERMISSIONS_REQUEST_LOCATION);
            }
        }

        private void ShowSettingsAlert()
        {
            AndroidX.AppCompat.App.AlertDialog.Builder alertDialog = new AndroidX.AppCompat.App.AlertDialog.Builder(this);
            alertDialog.SetTitle("Permission Required");
            alertDialog.SetMessage("This app needs location permissions to function. Please enable them in settings.");
            alertDialog.SetPositiveButton("Settings", delegate
            {
                Intent intent = new Intent(Android.Provider.Settings.ActionApplicationDetailsSettings,
                Android.Net.Uri.FromParts("package", PackageName, null));
                intent.AddFlags(ActivityFlags.NewTask);
                StartActivity(intent);
            });
            alertDialog.SetNegativeButton("Cancel", delegate
            {
                alertDialog.Dispose();
            });
            alertDialog.Show();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            if (requestCode == MY_PERMISSIONS_REQUEST_LOCATION)
            {
                if (grantResults.Length > 0 && grantResults[0] == Permission.Granted)
                {
                    // Permission was granted.
                }
                else
                {
                    // Permission denied.
                    ShowSettingsAlert(); // Prompt user to go to settings if needed
                }
            }
        }


        /*public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
           
        }*/


    }
}
