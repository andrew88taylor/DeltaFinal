using Firebase.Database;
using Newtonsoft.Json;
using Microsoft.Maui.Controls;
using System;
using System.Threading.Tasks;
using Firebase.Database.Query;
using System.Diagnostics;

namespace Delta
{
    public partial class Profile : ContentPage
    {
        FirebaseClient firebaseClient = new FirebaseClient(baseUrl: "https://delt-b80a2-default-rtdb.firebaseio.com/");

        public Profile()
        {
            InitializeComponent();
            FetchProfileInfo();
            GenderPicker.SelectedIndexChanged += OnGenderSelected;
        }

        private async Task FetchProfileInfo()
        {
            var userInfo = JsonConvert.DeserializeObject<Firebase.Auth.FirebaseAuth>(Preferences.Get("FreshFirebaseToken", ""));
            var sanitizedEmail = userInfo.User.Email.Replace('.', ',');

            var profileData = await firebaseClient.Child(sanitizedEmail).Child("Profile").OnceSingleAsync<UserProfileInfo>();

            if (profileData != null)
            {
                GenderPicker.SelectedItem = profileData.Gender;
                HeightEntry.Text = profileData.Height.ToString();
                WeightEntry.Text = profileData.Weight.ToString();
                AgeEntry.Text = profileData.Age.ToString();
                WaistEntry.Text = profileData.WaistMeasurement.ToString();
                NeckEntry.Text = profileData.NeckMeasurement.ToString();
                HipEntry.Text = profileData.HipMeasurement.ToString();

                HipEntryLayout.IsVisible = profileData.Gender == "Female";

                DisplayInfo(profileData);
            }
        }

        private void OnGenderSelected(object sender, EventArgs e)
        {
            var selectedGender = GenderPicker.SelectedItem as string;
            HipEntryLayout.IsVisible = selectedGender == "Female";
        }

        private async void OnSaveProfileButtonClicked(object sender, System.EventArgs e)
        {
            // Validation - Check if all fields are filled
            if (GenderPicker.SelectedItem == null ||
                string.IsNullOrWhiteSpace(HeightEntry.Text) ||
                string.IsNullOrWhiteSpace(WeightEntry.Text) ||
                string.IsNullOrWhiteSpace(AgeEntry.Text) ||
                string.IsNullOrWhiteSpace(WaistEntry.Text) ||
                string.IsNullOrWhiteSpace(NeckEntry.Text) ||
                (GenderPicker.SelectedItem.ToString() == "Female" && string.IsNullOrWhiteSpace(HipEntry.Text)))
            {
                await DisplayAlert("Alert", "Please fill in all fields", "OK");
                return;
            }

            var userInfo = JsonConvert.DeserializeObject<Firebase.Auth.FirebaseAuth>(Preferences.Get("FreshFirebaseToken", ""));
            var sanitizedEmail = userInfo.User.Email.Replace('.', ',');

            var profileInfo = new UserProfileInfo
            {
                Gender = GenderPicker.SelectedItem.ToString(),
                Height = int.Parse(HeightEntry.Text),
                Weight = int.Parse(WeightEntry.Text),
                Age = int.Parse(AgeEntry.Text),
                WaistMeasurement = int.Parse(WaistEntry.Text),
                NeckMeasurement = int.Parse(NeckEntry.Text),
                HipMeasurement = GenderPicker.SelectedItem.ToString() == "Female" ? int.Parse(HipEntry.Text) : 0
            };

            await firebaseClient.Child(sanitizedEmail).Child("Profile").PutAsync(profileInfo);

            DisplayInfo(profileInfo);
        }


        private void DisplayInfo(UserProfileInfo profileData)
        {
            double bmi = CalculateBMI(profileData.Weight, profileData.Height);
            double bodyFatPercentage = CalculateBodyFatPercentage(profileData.Age, profileData.WaistMeasurement, profileData.NeckMeasurement, profileData.Height, profileData.HipMeasurement, profileData.Gender);

            DisplayInfoLabel.Text = $"EST BMI: {bmi:F2}\nEST Body Fat Percentage: {bodyFatPercentage:F2}%";
        }


        private double CalculateBMI(int weight, int height)
        {
            double heightInMeters = height * 0.0254;
            double weightInKg = weight * 0.453592;
            return weightInKg / (heightInMeters * heightInMeters);
        }

        private double CalculateBodyFatPercentage(int age, int waist, int neck, int height, int hip, string gender)
        {
            if (gender == "Male")
            {
                return 86.010 * Math.Log10(waist - neck) - 70.041 * Math.Log10(height) + 36.76;
            }
            else if (gender == "Female")
            {
                return 163.205 * Math.Log10(waist + hip - neck) - 97.684 * Math.Log10(height) - 78.387;
            }
            else // For unspecified or other genders
            {
                // You can either take an average of male and female formulas, 
                // or default to one of them, or implement a different logic for unspecified genders.
                double maleFormula = 86.010 * Math.Log10(waist - neck) - 70.041 * Math.Log10(height) + 36.76;
                double femaleFormula = 163.205 * Math.Log10(waist + hip - neck) - 97.684 * Math.Log10(height) - 78.387;
                return (maleFormula + femaleFormula) / 2;
            }
        }
        private async void OnSignOutButtonClicked(object sender, EventArgs e)
        {
            try
            {
                var auth = JsonConvert.DeserializeObject<Firebase.Auth.FirebaseAuth>(Preferences.Get("FreshFirebaseToken", ""));
                // Here, add the code for signing out using the `auth` instance.
                // This might be something like: await auth.SignOutAsync();

                // Clear any stored user authentication information
                Preferences.Remove("FreshFirebaseToken");

                // Navigate to the login page or initial screen
                await Shell.Current.GoToAsync("//LoginViewModel");
            }
            catch (Exception ex)
            {
                // Log the exception
                Debug.WriteLine($"Error during sign out: {ex.Message}");
                // Optionally display an error message to the user
            }
        }


    }

    public class UserProfileInfo
    {
        public string Gender { get; set; }
        public int Height { get; set; }
        public int Weight { get; set; }
        public int Age { get; set; }
        public int WaistMeasurement { get; set; }
        public int NeckMeasurement { get; set; }
        public int HipMeasurement { get; set; }
    }
}


