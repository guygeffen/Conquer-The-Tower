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
    [Activity(Label = "UserProfileActivity")]
    public class UserProfileActivity : Activity
    {
        private EditText txtName;
        private EditText txtAge;
        private RadioGroup radioGroupGender;
        private EditText txtWeight;
        private EditText txtHeight;
        private UserProfileDbHelper _dbHelper;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our layout resource
            SetContentView(Resource.Layout.activity_user_properties);

            // Get references to UI elements
            txtName = FindViewById<EditText>(Resource.Id.txtName);
            txtAge = FindViewById<EditText>(Resource.Id.txtAge);
            radioGroupGender = FindViewById<RadioGroup>(Resource.Id.radioGroupGender);
            txtWeight = FindViewById<EditText>(Resource.Id.txtWeight);
            txtHeight = FindViewById<EditText>(Resource.Id.txtHeight);

            // Set weight input to accept decimals with one digit after the point
            txtWeight.InputType = Android.Text.InputTypes.ClassNumber;
            //txtWeight.v = "0.0-9";

            // Initialize database helper
            _dbHelper = UserProfileDbHelper.GetInstance();

            // Load existing user data and populate UI (if any)
            UserProfile profile = _dbHelper.GetUserProfile();
            if (profile != null)
            {
                txtName.Text = profile.Name;
                txtAge.Text=profile.Age.ToString(); 

                int checkedRadioButtonId;
                if (profile.Gender == "Female")
                {
                    checkedRadioButtonId = Resource.Id.radioButtonFemale;
                }
                else if (profile.Gender == "Other")
                {
                    checkedRadioButtonId = Resource.Id.radioButtonOther;
                }
                else
                {
                    checkedRadioButtonId = Resource.Id.radioButtonMale;
                }
                radioGroupGender.Check(checkedRadioButtonId);

                txtWeight.Text = profile.Weight.ToString();
                txtHeight.Text = profile.Height.ToString();
            }

            // Add button to save data
            var btnSave = FindViewById<Button>(Resource.Id.btnSave);
            btnSave.Click += BtnSave_Click; // Assign click handler

            // Add button to go back to Main Activity (optional, implement based on your navigation needs)
            // var btnBack = FindViewById<Button>(Resource.Id.btnBack);
            // btnBack.Click += BtnBack_Click; // Implement back button functionality
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            // Get user input
            string name = txtName.Text;
            int age = int.Parse(txtAge.Text);
            string gender = "";
            int checkedRadioButtonId = radioGroupGender.CheckedRadioButtonId;
            if (checkedRadioButtonId == Resource.Id.radioButtonMale)
            {
                gender = "Male";
            }
            else if (checkedRadioButtonId == Resource.Id.radioButtonFemale)
            {
                gender = "Female";
            }
            else if (checkedRadioButtonId == Resource.Id.radioButtonOther)
            {
                gender = "Other";
            }
            double weight = double.Parse(txtWeight.Text);
            int height = int.Parse(txtHeight.Text);

            // Create a user profile object
            UserProfile profile = new UserProfile(name, age, gender, weight, height);

            // Save user profile to database
            _dbHelper.SaveUserProfile(profile);

            // Optionally, send data back to MainActivity
            Intent intent = new Intent();
            intent.PutExtra("message", "User profile saved!"); // Example: send a message back to MainActivity

            SetResult(Result.Ok, intent); // Set result code and data (optional)
            Finish(); // Close this activity
        }
    }
}