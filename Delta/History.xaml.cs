using Firebase.Database;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using Microsoft.Maui.Controls;
using System.Threading.Tasks;
using System.Linq;
using Firebase.Database.Query;


namespace Delta
{
    public partial class History : ContentPage
    {
        private FirebaseClient firebaseClient;
        private ObservableCollection<WorkoutViewModel> workouts;

        public History()
        {
            InitializeComponent();
            firebaseClient = new FirebaseClient("https://delt-b80a2-default-rtdb.firebaseio.com/");
            workouts = new ObservableCollection<WorkoutViewModel>();
            HistoryListView.ItemsSource = workouts;
            FetchWorkouts();
        }

        private async void FetchWorkouts()
        {
            try
            {
                var userInfo = JsonConvert.DeserializeObject<Firebase.Auth.FirebaseAuth>(Preferences.Get("FreshFirebaseToken", ""));
                string userEmail = userInfo.User.Email.Replace(".", ",");

                var workoutData = await firebaseClient
                    .Child(userEmail)
                    .Child("Workouts")
                    .OnceAsync<Workout>();

                foreach (var item in workoutData)
                {
                    var workout = item.Object;

                    var exercises = new List<Exercise>();
                    foreach (var exerciseEntry in workout.Exercises)
                    {
                        var exercise = new Exercise
                        {
                            Type = exerciseEntry.Key,
                            CustomFields = exerciseEntry.Value.Select(cf => new CustomField { FieldName = cf.Key, FieldValue = cf.Value }).ToList()
                        };
                        exercises.Add(exercise);
                    }

                    workouts.Add(new WorkoutViewModel
                    {
                        Date = workout.Date,
                        Exercises = new ObservableCollection<Exercise>(exercises)
                    });
                    // Debugging: Print workout details
                    Console.WriteLine($"Workout Date: {workout.Date}, Exercises: {exercises.Count}");
                }
                Console.WriteLine($"Total workouts fetched: {workouts.Count}");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }
    }

    public class WorkoutViewModel
    {
        public DateTime Date { get; set; }
        public ObservableCollection<Exercise> Exercises { get; set; }
    }
}
