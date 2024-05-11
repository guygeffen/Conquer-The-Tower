using Android.App;
using Android.OS;
using Android.Widget;
using AndroidX.AppCompat.App;
using Google.Android.Material.FloatingActionButton;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Locations;
using Android.Content;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.Json;

namespace CttApp
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class PlayGameActivity : AppCompatActivity, IOnMapReadyCallback, ILocationListener
    {
        private MapView mapView;
        private GoogleMap googleMap;
        private FloatingActionButton aimButton;
        private FloatingActionButton pauseButton;
        private FloatingActionButton stopButton;
        private TextView timeLabel;
        private Game game;
        LocationManager locationManager;
        public static string GAME_RESULT = "gameResult";
        private List<Marker> towerMarkers = new List<Marker>();


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_play_game); // Make sure your layout file is correctly named
            InitializeViews(savedInstanceState);
            InitializeMap(savedInstanceState);
            locationManager = (LocationManager)GetSystemService(LocationService);
            

        }
        void RequestLocationUpdates()
        {
            var provider = LocationManager.GpsProvider;
            if (locationManager.IsProviderEnabled(provider))
            {
                locationManager.RequestLocationUpdates(provider, 2000, 1, this);
            }
            else
            {
                // Provider not enabled, prompt user to enable it
                Toast.MakeText(this, $"{provider} is not enabled.", ToastLength.Long).Show();
            }
        }

        public void OnLocationChanged(Android.Locations.Location location)
        {
            if (googleMap != null)
            {
                LatLng newLatLng = new LatLng(location.Latitude, location.Longitude);
                

                Location centerLocation = new Location(location.Latitude, location.Longitude, location.Altitude);

                if (this.game == null)
                {
                    Task.Run(async () => {
                        this.game = await Game.CreateAsync(centerLocation, 100, Intent.GetIntExtra(NewGameActivity.num_towers_key, 1),
                            Intent.GetIntExtra(NewGameActivity.game_radius_key, 500), Intent.GetIntExtra(NewGameActivity.game_time_key, 5));
                        List<Tower> towers = game.GetTowers();

                        RunOnUiThread(() => {
                            foreach (Marker marker in towerMarkers)
                            {
                                marker.Remove();
                            }
                            towerMarkers.Clear();

                            foreach (Tower tower in towers)
                            {
                                Location towerLocation = tower.Location;
                                LatLng towerLatLng = new LatLng(towerLocation.Latitude, towerLocation.Longitude);
                                Marker marker = googleMap.AddMarker(new MarkerOptions().SetPosition(towerLatLng).SetTitle("Tower"));
                                towerMarkers.Add(marker);
                            }
                            googleMap.MoveCamera(CameraUpdateFactory.NewLatLngZoom(newLatLng, MapHelper.EstimateZoomLevel(Intent.GetIntExtra(NewGameActivity.game_radius_key, 500))));
                        });
                    }).ContinueWith(task => {
                        if (task.IsFaulted)
                        {
                            Android.Util.Log.Error("PlayGameActivity", $"Error initializing game: {task.Exception.InnerException.Message}\nStack trace: {task.Exception.InnerException.StackTrace}");
                        }
                    });
                }
                else
                {
                    GameResult gameResult = this.game.Update(centerLocation);
                    if (gameResult.IsGameEnded())
                    {
                        Intent returnIntent = new Intent();
                        returnIntent.PutExtra(GAME_RESULT, JsonSerializer.Serialize(gameResult));
                        SetResult(Result.Ok, returnIntent);
                        Finish();
                    }
                    else
                    {
                        timeLabel.Text = game.GetTimeLeftInMinutes().ToString("F2");
                    }
                    googleMap.MoveCamera(CameraUpdateFactory.NewLatLng(newLatLng));
                }
            }
        }


        public void OnProviderDisabled(string provider)
        {
            Toast.MakeText(this, $"{provider} disabled by user", ToastLength.Long).Show();
        }

        public void OnProviderEnabled(string provider)
        {
            Toast.MakeText(this, $"{provider} enabled by user", ToastLength.Long).Show();
        }

        public void OnStatusChanged(string provider, Availability status, Bundle extras)
        {
            // This method is called when the status of the provider changes.
        }

        private void InitializeViews(Bundle savedInstanceState)
        {
            mapView = FindViewById<MapView>(Resource.Id.gameMap);
            mapView.OnCreate(savedInstanceState);

            aimButton = FindViewById<FloatingActionButton>(Resource.Id.aimButton);
            pauseButton = FindViewById<FloatingActionButton>(Resource.Id.pauseButton);
            stopButton = FindViewById<FloatingActionButton>(Resource.Id.stopButton);
            timeLabel = FindViewById<TextView>(Resource.Id.timeLabel);

            aimButton.Click += AimButton_Click;
            pauseButton.Click += PauseButton_Click;
            stopButton.Click += StopButton_Click;
        }

        private void InitializeMap(Bundle savedInstanceState)
        {
            mapView.GetMapAsync(this);
        }

        public void OnMapReady(GoogleMap map)
        {
            googleMap = map;
            SetupMap();
            RequestLocationUpdates();
        }

        private void SetupMap()
        {
            // Customize your map here
            googleMap.MapType = GoogleMap.MapTypeNormal;
            googleMap.MyLocationEnabled = true;
        }

        private void AimButton_Click(object sender, System.EventArgs e)
        {
            // Implement action for aim button
        }

        private void PauseButton_Click(object sender, System.EventArgs e)
        {
            // Implement action for pause button
        }

        private void StopButton_Click(object sender, System.EventArgs e)
        {
            Intent intent = new Intent();
            SetResult(Result.Canceled, intent); // Set result code and data (optional)
            Finish(); // Close this activity
        }

        protected override void OnResume()
        {
            base.OnResume();
            mapView.OnResume();
            RequestLocationUpdates();
        }

        protected override void OnPause()
        {
            base.OnPause();
            mapView.OnPause();
            locationManager.RemoveUpdates(this);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            mapView.OnDestroy();
            locationManager.RemoveUpdates(this);
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);
            mapView.OnSaveInstanceState(outState);
        }

        public override void OnLowMemory()
        {
            base.OnLowMemory();
            mapView.OnLowMemory();
        }

        
    }
}
