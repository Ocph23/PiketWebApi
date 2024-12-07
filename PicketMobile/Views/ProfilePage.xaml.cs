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
    public ICommand LogoutCommand { get; set; }
    public ProfilePageViewModel()
    {
		LogoutCommand = new RelayCommand(() => {
			Preferences.Set("token", null);

			App.Current.MainPage = new LoginPage();
		
		});
    }
}