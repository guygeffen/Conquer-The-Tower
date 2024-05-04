using Android.App;
using Android.OS;
using Android.Widget;
using AndroidX.AppCompat.App;
using Google.Android.Material.FloatingActionButton;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;

namespace CttApp
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class PlayGameActivity : AppCompatActivity, IOnMapReadyCallback
    {
        private MapView mapView;
        private GoogleMap googleMap;
        private FloatingActionButton aimButton;
        private FloatingActionButton pauseButton;
        private FloatingActionButton stopButton;
        private TextView timeLabel;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_play_game); // Make sure your layout file is correctly named

            InitializeViews(savedInstanceState);
            InitializeMap(savedInstanceState);
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
        }

        private void SetupMap()
        {
            // Customize your map here
            googleMap.MapType = GoogleMap.MapTypeNormal;
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
            // Implement action for stop button
        }

        protected override void OnResume()
        {
            base.OnResume();
            mapView.OnResume();
        }

        protected override void OnPause()
        {
            base.OnPause();
            mapView.OnPause();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            mapView.OnDestroy();
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
