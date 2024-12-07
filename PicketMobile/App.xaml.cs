using PicketMobile.Views;

namespace PicketMobile
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            string token = Preferences.Get(key: "token", null);
            if (string.IsNullOrEmpty(token))
            {
                return new Window(new LoginPage());
            }
            else
            {
                return new Window(new AppShell());
            }
        }
    }
}