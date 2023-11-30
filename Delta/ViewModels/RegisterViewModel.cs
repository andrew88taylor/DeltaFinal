using Firebase.Auth;
using System.ComponentModel;
using System.Linq; 

namespace Delta.ViewModels
{
    internal class RegisterViewModel : INotifyPropertyChanged
    {
        public string webApiKey = "AIzaSyDxRnVuD4LCsH8OC12R3ZRkpPvqtXTdt-w";

        private INavigation _navigation;
        private string email;
        private string password;

        public event PropertyChangedEventHandler PropertyChanged;

        public string Email
        {
            get => email;
            set
            {
                email = value;
                RaisePropertyChanged("Email");
            }
        }

        public string Password
        {
            get => password;
            set
            {
                password = value;
                RaisePropertyChanged("Password");
            }
        }

        public Command RegisterUser { get; }

        private void RaisePropertyChanged(string v)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(v));
        }

        public RegisterViewModel(INavigation navigation)
        {
            this._navigation = navigation;
            RegisterUser = new Command(RegisterUserTappedAsync);
        }

        private async void RegisterUserTappedAsync(object obj)
        {
            // Check if email or password is empty
            if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Password))
            {
                await App.Current.MainPage.DisplayAlert("Alert", "Please fill in both fields", "OK");
                return;
            }

            // Check if password meets criteria (at least 6 characters and contains at least one uppercase letter)
            if (Password.Length < 6 || !Password.Any(char.IsUpper))
            {
                await App.Current.MainPage.DisplayAlert("Alert", "Password must be at least 6 characters long and contain at least one uppercase letter and number or special character", "OK");
                return;
            }

            try
            {
                var authProvider = new FirebaseAuthProvider(new FirebaseConfig(webApiKey));
                var auth = await authProvider.CreateUserWithEmailAndPasswordAsync(Email, Password);
                string token = auth.FirebaseToken;
                if (token != null)
                {
                    await App.Current.MainPage.DisplayAlert("Alert", "User Registered successfully", "OK");
                    // Navigating back to login page after successful registration
                    await this._navigation.PopAsync();
                }
            }
            catch (FirebaseAuthException fae)
            {
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Alert", ex.Message, "OK");
                throw;
            }
        }
    }
}

