using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using System;

namespace CttApp
{
    [Activity(Label = "AimActivity")]
    public class AimActivity : Activity
    {
        private TextView textViewAzimuth;
        private SeekBar seekBarAzimuth;
        private TextView textViewInclination;
        private SeekBar seekBarInclination;
        private Button buttonSetAim;
        private Button buttonShoot;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_aim);

            textViewAzimuth = FindViewById<TextView>(Resource.Id.textViewAzimuth);
            seekBarAzimuth = FindViewById<SeekBar>(Resource.Id.seekBarAzimuth);
            textViewInclination = FindViewById<TextView>(Resource.Id.textViewInclination);
            seekBarInclination = FindViewById<SeekBar>(Resource.Id.seekBarInclination);
            buttonSetAim = FindViewById<Button>(Resource.Id.buttonSetAim);
            buttonShoot = FindViewById<Button>(Resource.Id.buttonShoot);

            // Retrieve the initial values from the Intent
            double initialAzimuth = Intent.GetDoubleExtra("Azimuth", 0);
            double initialInclination = Intent.GetDoubleExtra("Inclination", 0);

            // Set the initial values to the SeekBars and TextViews
            seekBarAzimuth.Progress = (int)initialAzimuth;
            seekBarInclination.Progress = (int)initialInclination;
            textViewAzimuth.Text = $"Azimuth: {initialAzimuth}°";
            textViewInclination.Text = $"Inclination: {initialInclination}°";

            seekBarAzimuth.ProgressChanged += SeekBarAzimuth_ProgressChanged;
            seekBarInclination.ProgressChanged += SeekBarInclination_ProgressChanged;

            buttonSetAim.Click += ButtonSetAim_Click;
            buttonShoot.Click += ButtonShoot_Click;
        }

        private void SeekBarAzimuth_ProgressChanged(object sender, SeekBar.ProgressChangedEventArgs e)
        {
            textViewAzimuth.Text = $"Azimuth: {e.Progress}°";
        }

        private void SeekBarInclination_ProgressChanged(object sender, SeekBar.ProgressChangedEventArgs e)
        {
            textViewInclination.Text = $"Inclination: {e.Progress}°";
        }

        private void ButtonSetAim_Click(object sender, EventArgs e)
        {
            Intent resultIntent = new Intent();
            resultIntent.PutExtra("Azimuth", (double)seekBarAzimuth.Progress);
            resultIntent.PutExtra("Inclination", (double)seekBarInclination.Progress);
            resultIntent.PutExtra("ButtonClicked", "Set Aim");

            SetResult(Result.Ok, resultIntent);
            Finish();
        }

        private void ButtonShoot_Click(object sender, EventArgs e)
        {
            Intent resultIntent = new Intent();
            resultIntent.PutExtra("Azimuth", (double)seekBarAzimuth.Progress);
            resultIntent.PutExtra("Inclination", (double)seekBarInclination.Progress);
            resultIntent.PutExtra("ButtonClicked", "Shoot");

            SetResult(Result.Ok, resultIntent);
            Finish();
        }
    }
}
