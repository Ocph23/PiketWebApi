using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;

namespace PicketMobile.Views;

public partial class ProfilePage : ContentPage
{
	public ProfilePage()
	{
		InitializeComponent();
		BindingContext = new ProfilePageViewModel();
	}
}

internal class ProfilePageViewModel
{
    public AsyncRelayCommand LogoutCommand { get; set; }
    public ProfilePageViewModel()
    {
		LogoutCommand = new AsyncRelayCommand(LogoutAction);
    }

    private Task LogoutAction()
    {
        Preferences.Set("token", null);
        App.Current.MainPage = new LoginPage();
        return Task.CompletedTask;
    }
}