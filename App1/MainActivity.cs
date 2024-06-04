using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.Core.Content;
using AndroidX.Core.App;
using Android.Views;
using Android;
using Android.Content.PM;
using Android.Runtime;

namespace CttApp
{
    /// <summary>
    /// Main activity of the application, handles navigation to other activities and location permissions.
    /// </summary>
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        private const int UserProfileRequestCode = 1; // Request code for UserProfileActivity
        private const int NewGameRequestCode = 2; // Request code for NewGameActivity
        private const int PlayGameRequestCode = 3; // Request code for UserProfileActivity
        private const int MY_PERMISSIONS_REQUEST_LOCATION = 1000;

        /// <summary>
        /// Called when the activity is first created.
        /// </summary>
        /// <param name="savedInstanceState">The saved instance state.</param>
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            // Find the button with ID "userPropButton"
            Button userPropButton = FindViewById<Button>(Resource.Id.userPropButton);

            // Assign a click handler to the button
            userPropButton.Click += UserPropButton_Click;

            // Find the button with ID "newGameButton"
            Button newGameButton = FindViewById<Button>(Resource.Id.newGameButton);

            // Assign a click handler to the button
            newGameButton.Click += NewGameButton_Click;

            // Check location permission
            if (!CheckLocationPermissions())
            {
                RequestLocationPermissions();
            }
        }

        /// <summary>
        /// Handles the click event for the new game button.
        /// </summary>
        private void NewGameButton_Click(object sender, EventArgs e)
        {
            // Create an Intent to launch NewGameActivity
            Intent intent = new Intent(this, typeof(NewGameActivity));

            // Start the activity for result
            StartActivityForResult(intent, NewGameRequestCode);
        }

        /// <summary>
        /// Handles the click event for the user properties button.
        /// </summary>
        private void UserPropButton_Click(object sender, EventArgs e)
        {
            // Create an Intent to launch UserProfileActivity
            Intent intent = new Intent(this, typeof(UserProfileActivity));

            // Start the activity for result
            StartActivityForResult(intent, UserProfileRequestCode);
        }

        /// <summary>
        /// Called when an activity you launched exits, giving you the requestCode you started it with,
        /// the resultCode it returned, and any additional data from it.
        /// </summary>
        /// <param name="requestCode">The integer request code originally supplied to StartActivityForResult(), allowing you to identify who this result came from.</param>
        /// <param name="resultCode">The integer result code returned by the child activity through its SetResult().</param>
        /// <param name="data">An Intent, which can return result data to the caller (various data can be attached to Intent "extras").</param>
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
                // Handle potential data returned by NewGameActivity
                // (e.g., display a message or update UI based on saved information)
                StartActivityForResult(data, PlayGameRequestCode);
            }
        }

        /// <summary>
        /// Initialize the contents of the Activity's standard options menu.
        /// </summary>
        /// <param name="menu">The options menu in which you place your items.</param>
        /// <returns>boolean You must return true for the menu to be displayed; if you return false it will not be shown.</returns>
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        /// <summary>
        /// This hook is called whenever an item in your options menu is selected.
        /// </summary>
        /// <param name="item">The menu item that was selected.</param>
        /// <returns>boolean Return false to allow normal menu processing to proceed, true to consume it here.</returns>
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.action_settings)
            {
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        /// <summary>
        /// Checks if location permissions are granted.
        /// </summary>
        /// <returns>True if permissions are granted, false otherwise.</returns>
        private bool CheckLocationPermissions()
        {
            return ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessFineLocation) == (int)Permission.Granted &&
                   ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessCoarseLocation) == (int)Permission.Granted;
        }

        /// <summary>
        /// Requests location permissions from the user.
        /// </summary>
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

        /// <summary>
        /// Shows an alert dialog prompting the user to enable location permissions in settings.
        /// </summary>
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

        /// <summary>
        /// Callback for the result from requesting permissions. This method is invoked for every call on RequestPermissions(android.app.Activity, String[], int).
        /// </summary>
        /// <param name="requestCode">The request code passed in RequestPermissions(android.app.Activity, String[], int).</param>
        /// <param name="permissions">The requested permissions. Never null.</param>
        /// <param name="grantResults">The grant results for the corresponding permissions which is either PERMISSION_GRANTED or PERMISSION_DENIED. Never null.</param>
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
    }
}
