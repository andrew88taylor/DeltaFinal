using Firebase.Database;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using Microsoft.Maui.Controls;
using Firebase.Database.Query;
using System.Windows.Input;

namespace Delta
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Dashboard : ContentPage
    {
        FirebaseClient firebaseClient = new FirebaseClient(baseUrl: "https://delt-b80a2-default-rtdb.firebaseio.com/");
        public ObservableCollection<userInfo> TodoItems { get; set; } = new ObservableCollection<userInfo>();
        public ObservableCollection<Exercise> Exercises { get; set; } = new ObservableCollection<Exercise>();

        public Dashboard()
        {
            InitializeComponent();
            ExercisesListView.ItemsSource = Exercises;
        }

        private void OnDeleteExerciseClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is Exercise exercise)
            {
                Exercises.Remove(exercise);
            }
        }

        private async void AddExerciseButtonClicked(object sender, EventArgs e)
        {
            var exerciseInputPage = new ExerciseInputModalPage();
            exerciseInputPage.OnExerciseSaved += (exercise) =>
            {
                Exercises.Add(exercise);
            };
            await Navigation.PushModalAsync(exerciseInputPage);
        }


        private void GetProfileInfo()
        {
            var userInfo = JsonConvert.DeserializeObject<Firebase.Auth.FirebaseAuth>(Preferences.Get("FreshFirebaseToken", ""));
        }


        private async void SaveWorkoutClicked(object sender, EventArgs e)
        {
            try
            {
                var userInfo = JsonConvert.DeserializeObject<Firebase.Auth.FirebaseAuth>(Preferences.Get("FreshFirebaseToken", ""));
                string userEmail = userInfo.User.Email.Replace(".", ",");

                // Format the date to use as a key (for example, yyyy-MM-dd)
                string dateKey = DateTime.Now.ToString("yyyy-MM-dd_HH-mm");

                // Creating a dictionary to hold exercises and their custom fields
                var exercisesDict = new Dictionary<string, object>();
                foreach (var exercise in Exercises)
                {
                    exercisesDict.Add(exercise.Type, exercise.CustomFields.ToDictionary(cf => cf.FieldName, cf => cf.FieldValue));
                }

                // Construct the workout data
                var workoutData = new { Date = DateTime.Now, Exercises = exercisesDict };

                // Saving the workout data under the date key
                await firebaseClient
                    .Child(userEmail)
                    .Child("Workouts")
                    .Child(dateKey)
                    .PutAsync(workoutData);

                await DisplayAlert("Success", "Workout saved!", "OK");
                Exercises.Clear();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }


        private async void NavigateToProfileButtonClicked(object sender, EventArgs e)
        {
            try
            {
                await Shell.Current.GoToAsync("//Profile");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }
    }
}
