using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PicketMobile.Services;
using System.Windows.Input;

namespace PicketMobile.Views;

public partial class LoginPage : ContentPage
{
    public LoginPage()
    {
        InitializeComponent();
        BindingContext = new LoginPageViewModel();
    }
}

internal class LoginPageViewModel : BaseNotify
{

    public LoginPageViewModel()
    {

        LoginCommand = new AsyncRelayCommand(LoginCommandAction, LoginCommandValidate);
        this.PropertyChanged += (s, p) =>
        {
            if (p.PropertyName != "LoginCommand")
            {
                LoginCommand = new AsyncRelayCommand(LoginCommandAction, LoginCommandValidate);
            }
        };
    }

    private bool LoginCommandValidate()
    {
        if (string.IsNullOrEmpty(UserName) || string.IsNullOrEmpty(Password))
            return false;
        return true;
    }

    [Obsolete]
    private async Task LoginCommandAction()
    {
        try
        {
            if (IsBusy)
                return;

            IsBusy = true;

            IAccountService service = ServiceHelper.GetService<IAccountService>();
            var result = await service.Login(UserName, Password);
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private string userName;

    public string UserName
    {
        get { return userName; }
        set { SetProperty(ref userName, value); }
    }



    private string password;

    public string Password
    {
        get { return password; }
        set { SetProperty(ref password, value); }
    }




    private ICommand loginCommand;

    public ICommand LoginCommand
    {
        get { return loginCommand; }
        set { SetProperty(ref loginCommand, value); }
    }


}