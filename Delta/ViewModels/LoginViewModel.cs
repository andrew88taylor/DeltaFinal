using Firebase.Auth;
using Newtonsoft.Json;
using System;
using System.ComponentModel;
using Microsoft.Maui.Controls; // Updated namespace for .NET MAUI

namespace Delta.ViewModels
{
    internal class LoginViewModel : INotifyPropertyChanged
    {
        public string webApiKey = "AIzaSyDxRnVuD4LCsH8OC12R3ZRkpPvqtXTdt-w";
        private INavigation _navigation;
        public string userName;
        private string userPassword;

        public event PropertyChangedEventHandler PropertyChanged;

        public Command RegisterBtn { get; }
        public Command LoginBtn { get; }

        public string UserName
        {
            get => userName;
            set
            {
                userName = value;
                RaisePropertyChanged(nameof(UserName));
            }
        }

        public string UserPassword
        {
            get => userPassword;
            set
            {
                userPassword = value;
                RaisePropertyChanged(nameof(UserPassword));
            }
        }

        public LoginViewModel(INavigation navigation)
        {
            this._navigation = navigation;
            RegisterBtn = new Command(RegisterBtnTappedAsync);
            LoginBtn = new Command(LoginBtnTappedAsync);
        }

        private async void LoginBtnTappedAsync(object obj)
        {
            // Check if email and password fields are filled
            if (string.IsNullOrEmpty(UserName) || string.IsNullOrEmpty(UserPassword))
            {
                await App.Current.MainPage.DisplayAlert("Alert", "Please fill in both fields", "OK");
                return;
            }

            var authProvider = new FirebaseAuthProvider(new FirebaseConfig(webApiKey));
            try
            {
                var auth = await authProvider.SignInWithEmailAndPasswordAsync(UserName, UserPassword);
                var content = await auth.GetFreshAuthAsync();
                var serializedContent = JsonConvert.SerializeObject(content);
                Preferences.Set("FreshFirebaseToken", serializedContent);

                // Navigate to the main page or update the UI after successful login
                App.Current.MainPage = new AppShell(); // Adjust as needed for your app
            }
            catch (FirebaseAuthException fae)
            {
                // Detailed error handling for FirebaseAuthException
                if (fae.Reason == AuthErrorReason.WrongPassword || fae.Reason == AuthErrorReason.UserNotFound)
                {
                    await App.Current.MainPage.DisplayAlert("Alert", "Wrong email or password", "OK");
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Alert", fae.Message, "OK");
                }
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Alert", "An error occurred during login", "OK");
                // Consider logging the exception for further analysis
            }
        }

        private async void RegisterBtnTappedAsync(object obj)
        {
            await _navigation.PushAsync(new RegisterPage()); // Adjust to your registration page navigation
        }

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
