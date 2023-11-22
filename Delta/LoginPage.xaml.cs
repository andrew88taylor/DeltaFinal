using Delta.ViewModels;
using Firebase.Database;
using System.Collections.ObjectModel;


namespace Delta
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
            BindingContext = new LoginViewModel(Navigation);

        }
    }
}

