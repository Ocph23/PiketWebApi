using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SharedModel.Responses;
using System.Text.Json;
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

internal partial class ProfilePageViewModel  :ObservableObject
{
    [ObservableProperty]
    private TeacherResponse profile;


    [ObservableProperty]
    private string initial;


    public AsyncRelayCommand LogoutCommand { get; set; }
    public ProfilePageViewModel()
    {
        LogoutCommand = new AsyncRelayCommand(LogoutAction);
        var profileString = Preferences.Get("profile", null);
        if(!string.IsNullOrEmpty(profileString))
        {
            this.Profile = JsonSerializer.Deserialize<TeacherResponse>(profileString, Helper.JsonOption);
            this.Initial = Helper.GetInitial(Profile.Name);
        }
    }

    private Task LogoutAction()
    {
        Preferences.Set("profile", null);
        App.Current.MainPage = new LoginPage();
        return Task.CompletedTask;
    }
}