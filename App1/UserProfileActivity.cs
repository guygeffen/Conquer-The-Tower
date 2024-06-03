using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;

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
        private Spinner spinnerUsers;
        private UserProfileDbHelper _dbHelper;
        private List<UserProfile> _userProfiles;

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
            spinnerUsers = FindViewById<Spinner>(Resource.Id.spinnerUsers);
            var btnNewUser = FindViewById<Button>(Resource.Id.btnNewUser);
            var btnSave = FindViewById<Button>(Resource.Id.btnSave);

            // Initialize database helper
            _dbHelper = UserProfileDbHelper.GetInstance();

            // Load existing users into Spinner
            LoadUserProfiles();

            // Handle user selection from Spinner
            spinnerUsers.ItemSelected += SpinnerUsers_ItemSelected;

            // Handle new user creation
            btnNewUser.Click += BtnNewUser_Click;

            // Handle save button click
            btnSave.Click += BtnSave_Click;

            // Optionally, add button to go back to Main Activity
            var btnBack = FindViewById<Button>(Resource.Id.btnBack);
            btnBack.Click += BtnBack_Click;
        }

        private void LoadUserProfiles()
        {
            _userProfiles = _dbHelper.GetAllUserProfiles();
            var userNames = _userProfiles.Select(u => u.Name).ToList();
            userNames.Insert(0, "Select User"); // Default item

            var adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, userNames);
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spinnerUsers.Adapter = adapter;
        }

        private void SpinnerUsers_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            if (e.Position == 0)
            {
                // Clear input fields for new user creation
                ClearInputFields();
            }
            else
            {
                // Load selected user details
                var selectedUser = _userProfiles[e.Position - 1];
                PopulateUserDetails(selectedUser);
            }
        }

        private void BtnNewUser_Click(object sender, EventArgs e)
        {
            ClearInputFields();
            spinnerUsers.SetSelection(0); // Reset Spinner selection
        }
        private void BtnBack_Click(object sender, EventArgs e)
        {
            UserProfile.selectedProfile= GetUserProfileFromView();
            // Optionally, send data back to MainActivity
            Intent intent = new Intent();
            intent.PutExtra("message", "User profile Selected!"); // Example: send a message back to MainActivity

            SetResult(Result.Ok, intent); // Set result code and data (optional)
            Finish(); // Close this activity
        }

        private UserProfile GetUserProfileFromView()
        {
            try
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
                return new UserProfile(name, age, gender, weight, height);
            }
            catch (Exception)
            {
                // Handle other potential exceptions
                return null;
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
           
            // Save user profile to database
            _dbHelper.SaveUserProfile(GetUserProfileFromView());

            // Optionally, send data back to MainActivity
            Intent intent = new Intent();
            intent.PutExtra("message", "User profile saved!"); // Example: send a message back to MainActivity

            SetResult(Result.Ok, intent); // Set result code and data (optional)
            Finish(); // Close this activity
        }

        private void ClearInputFields()
        {
            txtName.Text = "";
            txtAge.Text = "";
            radioGroupGender.ClearCheck();
            txtWeight.Text = "";
            txtHeight.Text = "";
        }

        private void PopulateUserDetails(UserProfile profile)
        {
            txtName.Text = profile.Name;
            txtAge.Text = profile.Age.ToString();

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
    }
}
