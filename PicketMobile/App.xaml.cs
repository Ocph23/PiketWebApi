using PicketMobile.Views;

namespace PicketMobile
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            string token = Preferences.Get(key: "token", null);
            if (string.IsNullOrEmpty(token))
            {
                MainPage = new LoginPage();
            }
            else
            {
                MainPage = new AppShell();
            }



        }
    }
}
