
namespace Delta
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            SetInitialPage();
        }

        private void SetInitialPage()
        {
            MainPage = new NavigationPage(new LoginPage());
        }
    }
}

