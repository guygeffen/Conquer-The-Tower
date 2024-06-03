using Android.App;
using Android.OS;
using Android.Widget;
using AndroidX.AppCompat.App;
using Google.Android.Material.FloatingActionButton;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Locations;
using Android.Content;
using System.Collections.Generic;
using Android.Views;
using Google.Android.Material.Snackbar;
using AlertDialog = Android.App.AlertDialog;

namespace CttApp
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme")]
    public class PlayGameActivity : AppCompatActivity, IOnMapReadyCallback, ILocationListener
    {
        private MapView mapView;
        private GoogleMap googleMap;
        private FloatingActionButton shootButton;
        private FloatingActionButton aimButton;
        
        private FloatingActionButton stopButton;
        private TextView timeLabel;
        private TextView playersHealth;
        private TextView towersHealth;
        private TextView caloriesBurned;
        private Game game;
        private bool gameStarted;
        private bool gameEnded;
        LocationManager locationManager;
        public static string GAME_RESULT = "gameResult";
        private readonly List<Marker> towerMarkers = new List<Marker>();
        private readonly List<Circle> hitMarkers = new List<Circle>();
        private Circle playerHitMarker;
        
        private Marker _currentLocationMarker;
        private GamePlayCountDownCounter countDownTimer;
        private Android.Locations.Location lastKnownLocation = null;
        private Snackbar snackbar;
        private static readonly int AimRequestCode = 0;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_play_game); // Make sure your layout file is correctly named
            InitializeViews(savedInstanceState);
            InitializeMap();
            locationManager = (LocationManager)GetSystemService(LocationService);
            countDownTimer = new GamePlayCountDownCounter(Intent.GetIntExtra(NewGameActivity.game_time_key, 5) * 60000, (long)GameConstants.towerAimingTime*1000, this);
            View rootView = FindViewById(Android.Resource.Id.Content);
            InitializeSnackbar(rootView);


        }

        private void InitializeSnackbar(View rootView)
        {
            snackbar = Snackbar.Make(rootView, "Waiting for GPS Signal...", Snackbar.LengthIndefinite);
            snackbar.Show(); // Show the snackbar indefinitely
        }
        void RequestLocationUpdates()
        {
            var provider = LocationManager.GpsProvider;
            if (locationManager.IsProviderEnabled(provider))
            {
                locationManager.RequestLocationUpdates(provider, 5000, 5, this);
            }
            else
            {
                // Provider not enabled, prompt user to enable it
                Toast.MakeText(this, $"{provider} is not enabled.", ToastLength.Long).Show();
            }
        }

        private void UpdateMyLocationUI(Android.Locations.Location location)
        {
            if (googleMap == null) return;

            if (_currentLocationMarker != null)
                _currentLocationMarker.Remove();

            LatLng userLatLng = new LatLng(location.Latitude, location.Longitude);
            MarkerOptions markerOptions = new MarkerOptions()
                .SetPosition(userLatLng)
                .SetTitle(game.GetPlayer().Name)
                .SetSnippet(game.GetPlayer().ToString())
                .SetIcon(BitmapDescriptorFactory.FromResource(Resource.Drawable.blue_tank_55));  // Assuming you have a custom_location_icon.png in your drawable folder

            _currentLocationMarker = googleMap.AddMarker(markerOptions);
            _currentLocationMarker.SetAnchor(0.5f, 0.5f);
            _currentLocationMarker.Rotation = (float)game.GetPlayer().Weapon.Azimuth;
            googleMap.MoveCamera(CameraUpdateFactory.NewLatLng(userLatLng)); // Adjust zoom as needed
        }

        private void InitializeGame(Android.Locations.Location location)
        {

            LatLng newLatLng = new LatLng(location.Latitude, location.Longitude);
            Location centerLocation = new Location(location.Latitude, location.Longitude);

            UserProfile user=UserProfileDbHelper.GetInstance().GetUserProfile();

            this.game = Game.Create(centerLocation, GameConstants.PlayerHealth, user, Intent.GetIntExtra(NewGameActivity.num_towers_key, 1),
                           Intent.GetIntExtra(NewGameActivity.game_radius_key, 500), Intent.GetIntExtra(NewGameActivity.game_time_key, 5),
                           Intent.GetIntExtra(NewGameActivity.range_key, 5));
            
            List<Tower> towers = game.GetTowers();


            foreach (Marker marker in towerMarkers)
            {
                marker.Remove();
            }
            towerMarkers.Clear();

            BitmapDescriptor towerBD = BitmapDescriptorFactory.FromResource(Resource.Drawable.tower__red_55);

            foreach (Tower tower in towers)
            {
                
                LatLng towerLatLng = tower.Location.GetLatLng(); 
                Marker marker = googleMap.AddMarker(new MarkerOptions().SetPosition(towerLatLng).SetTitle(tower.Name).SetSnippet(tower.ToString()).SetIcon(towerBD));
                marker.SetAnchor(0.5f, 0.5f);
                marker.Rotation = (float)tower.Weapon.Azimuth;
                towerMarkers.Add(marker);
            }
            googleMap.MoveCamera(CameraUpdateFactory.NewLatLngZoom(newLatLng, MapHelper.EstimateZoomLevel(Intent.GetIntExtra(NewGameActivity.game_radius_key, 500))));
            
            if (snackbar != null && snackbar.IsShown)
            {
                snackbar.Dismiss(); // Dismiss the snackbar when location is updated
            }
            ShowStartGameDialog();


        }
        private void ShowStartGameDialog()
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(this);
            builder.SetTitle("Start Game");
            builder.SetMessage($"{game.GetPlayer().Name}, Are you ready to go?");
            builder.SetPositiveButton("Yes", (sender, e) =>
            {
                // Start the game timer
                game.StartGame();
                RequestLocationUpdates();
                countDownTimer.Start();
                gameStarted = true;

            });

            AlertDialog dialog = builder.Create();
            dialog.Show();
        }

        private void HandleGamePlay(Location playerLocation)
        {
            GameResult gameResult = this.game.Update(playerLocation);

            if (!gameResult.IsGameEnded())
            {

                foreach (ShootingEntityHitResults hitResults in gameResult.HitResults)
                {
                    
                    Tower tower = (Tower)hitResults.Entity;
                    towerMarkers[tower.Index].Rotation = (float)hitResults.Entity.Weapon.Azimuth;
                    if (hitResults.HitResults != null && hitResults.HitResults.Count>0)
                    {
                        //remove previous hits if the first shot was shot
                        if(hitResults.HitResults[0].HitLocation !=null)
                        {
                            foreach (Circle hitMarker in hitMarkers)
                            {
                                hitMarker.Remove();
                            }
                            hitMarkers.Clear();
                        }
                       
                        foreach (HitResult hit in hitResults.HitResults)
                        {
                            if (hit.HitLocation != null)
                            {
                                CircleOptions circleOptions = new CircleOptions()
                                .InvokeFillColor(0x44FF0000) // Semi-transparent red fill (ARGB)
                                .InvokeStrokeColor(Android.Graphics.Color.Red) // Red border
                                .InvokeStrokeWidth(2); // Border width
                                LatLng hitLatLng = new LatLng(hit.HitLocation.Latitude, hit.HitLocation.Longitude);
                                circleOptions.InvokeRadius(hit.HitRadius).InvokeCenter(hitLatLng);
                                Circle circle = googleMap.AddCircle(circleOptions);
                                hitMarkers.Add(circle);
                            }

                        }
                    }
                }
                
            }

            else
            {
                gameEnded = true;
                if (gameResult.GetLeader() == GameResult.Leader.player)
                {
                    Toast.MakeText(this, $"Congartulations {game.GetPlayer().Name}, you've won", ToastLength.Long).Show();
                }
                else if (gameResult.GetLeader() == GameResult.Leader.computer)
                {
                    Toast.MakeText(this, "Too Bad, you've lost", ToastLength.Long).Show();
                }
                else if (gameResult.GetLeader() == GameResult.Leader.tie)
                {
                    Toast.MakeText(this, "You tied, get untied!", ToastLength.Long).Show();
                }
            }

            playersHealth.Text = $"{(int)game.GetPlayer().Health}";
            caloriesBurned.Text = $"{(int)game.GetPlayer().CaloriesBurned}";
            UpdateTowerLabels();
            UpdateMyLocationUI(lastKnownLocation);
            CheckWeaponCanShoot();
        }
        public void OnLocationChanged(Android.Locations.Location location)
        {
            if (location != null)
            {
                locationManager.RemoveUpdates(this);
                
                lastKnownLocation = location;

                Location centerLocation = new Location(location.Latitude, location.Longitude);
                if (this.game == null && !gameStarted && !gameEnded)
                {
                    InitializeGame(location);
                }
                else if (!gameEnded)
                {
                    HandleGamePlay(centerLocation);
                    RequestLocationUpdates();
                }
            }
        }


        private class GamePlayCountDownCounter : CountDownTimer
        {
            readonly PlayGameActivity activity;
            public override void OnFinish()
            {
                activity.timeLabel.Text = "Time Left: 00:00:00";
            }

            public override void OnTick(long millisUntilFinished)
            {
                int seconds = (int)(millisUntilFinished / 1000);  // Convert milliseconds to seconds
                int hours = seconds / 3600;                        // Convert seconds to hours
                int minutes = (seconds % 3600) / 60;               // Convert remainder to minutes
                seconds %= 60;                            // Remaining seconds

                // Format the string as HH:mm:ss
                if (!this.activity.gameEnded)
                {
                    activity.timeLabel.Text = "Time Left: " + $"{hours:00}:{minutes:00}:{seconds:00}";
                }
                
                if (activity.lastKnownLocation != null)
                {
                    activity.HandleGamePlay(activity.game.GetPlayer().Location);
                }
            }

            public GamePlayCountDownCounter(long length, long interval, PlayGameActivity activity) : base(length, interval)
            {
                this.activity = activity;
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

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (requestCode == AimRequestCode && resultCode == Result.Ok)
            {
                double azimuth = data.GetDoubleExtra("Azimuth", 0);
                double inclination = data.GetDoubleExtra("Inclination", 45);
                game.GetPlayer().Weapon.Azimuth = azimuth;
                game.GetPlayer().Weapon.Inclination = inclination;
                _currentLocationMarker.Rotation = (float)game.GetPlayer().Weapon.Azimuth;
                if (Intent.GetStringExtra("ButtonClicked") == "Shoot")
                {
                    PlayerShoot();
                }
            }


        }
        private void InitializeViews(Bundle savedInstanceState)
        {
            mapView = FindViewById<MapView>(Resource.Id.gameMap);
            mapView.OnCreate(savedInstanceState);

            shootButton = FindViewById<FloatingActionButton>(Resource.Id.shootButton);
            aimButton = FindViewById<FloatingActionButton>(Resource.Id.aimButton);
            
            stopButton = FindViewById<FloatingActionButton>(Resource.Id.stopButton);
            timeLabel = FindViewById<TextView>(Resource.Id.timeLabel);

            playersHealth = FindViewById<TextView>(Resource.Id.playerHealth);
            towersHealth = FindViewById<TextView>(Resource.Id.towerHealth);
            caloriesBurned = FindViewById<TextView>(Resource.Id.caloriesBurned);

            shootButton.Click += ShootButton_Click;
            aimButton.Click += AimButton_Click;
            
            stopButton.Click += StopButton_Click;
        }

        private void PlayerShoot()
        {
            if (gameStarted && !gameEnded)
            {
                if (playerHitMarker != null)
                {
                    playerHitMarker.Remove();
                }
                List<HitResult> hitResults = game.GetPlayer().Weapon.Fire(new List<Entity>(game.GetTowers()), new Shell((GameConstants.PlayerHealth/this.game.GetTowers().Count)/3, GameConstants.PlayerShellDamageRadius));
                if (hitResults != null)
                {
                    foreach (HitResult hit in hitResults)
                    {
                        if (hit.HitLocation != null)
                        {
                            CircleOptions circleOptions = new CircleOptions()
                            .InvokeFillColor(0x7F0000FF) // Semi-transparent blue fill (ARGB)
                            .InvokeStrokeColor(Android.Graphics.Color.Blue) // Red border
                            .InvokeStrokeWidth(2); // Border width
                            LatLng hitLatLng = new LatLng(hit.HitLocation.Latitude, hit.HitLocation.Longitude);
                            circleOptions.InvokeRadius(hit.HitRadius).InvokeCenter(hitLatLng);
                            playerHitMarker = googleMap.AddCircle(circleOptions);
                        }

                    }
                    UpdateTowerLabels();
                }
                towersHealth.Text= $"{(int)game.GetTowersHealth()}";
            }
        }

        private void UpdateTowerLabels()
        {
            for (int i = 0; i < towerMarkers.Count; i++)
            {
                Tower tower = game.GetTowers()[i];
                Marker towerMarker = towerMarkers[i];
                towerMarker.Snippet = tower.ToString();
                towerMarker.Title = tower.Name;
            }
        }


        private void ShootButton_Click(object sender, System.EventArgs e)
        {
            PlayerShoot();
        }

        private void InitializeMap()
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
            //googleMap.BuildingsEnabled = true;

            //add info window
            googleMap.SetInfoWindowAdapter(new CustomInfoWindowAdapter(LayoutInflater.From(this)));
        }

        private void AimButton_Click(object sender, System.EventArgs e)
        {
            Intent intent = new Intent(this, typeof(AimActivity));
            intent.PutExtra("Azimuth", game.GetPlayer().Weapon.Azimuth);
            intent.PutExtra("Inclination", game.GetPlayer().Weapon.Inclination);

            // Start the activity for result
            StartActivityForResult(intent, AimRequestCode);
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
            if (countDownTimer != null)
            {
                countDownTimer.Cancel(); // Ensure to cancel the timer if the activity is destroyed
            }
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

        private void CheckWeaponCanShoot()
        {
            RunOnUiThread(() => {
                bool canShoot = game.GetPlayer().Weapon.CanShoot() && !game.GameEnded;
                shootButton.Clickable = canShoot;
                shootButton.Alpha = canShoot ? 1f : 0.5f;
            });
        }


    }
}
