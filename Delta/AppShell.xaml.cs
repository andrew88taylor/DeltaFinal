using Microsoft.Maui.Controls;

namespace Delta
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Register the route for the Profile page
            Routing.RegisterRoute("Profile", typeof(Profile));

            // Register the route for the Dashboard page
            Routing.RegisterRoute("Dashboard", typeof(Dashboard));

            // Register the route for the History page
            Routing.RegisterRoute("History", typeof(History));
        }
    }
}

