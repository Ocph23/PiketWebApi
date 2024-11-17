using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PicketMobile.Services;
using SharedModel.Models;
using System.Windows.Input;

namespace PicketMobile.Views.Pickets;

public partial class PicketPage : ContentPage
{
    public PicketPage()
    {
        InitializeComponent();
        BindingContext = new PicketPageViewModel();
    }


}


internal partial class PicketPageViewModel : ObservableObject
{
    [ObservableProperty]
    Picket picket;

    [ObservableProperty]
    private bool canSync;

    [ObservableProperty]
    private bool hasPicket;

    [ObservableProperty]
    private bool iamPicket;


    [ObservableProperty]
    private string message = "";

    [ObservableProperty]
    ICommand addCommand;


    public Picket Model
    {

        get { return picket; }
        set { SetProperty(ref picket, value); }
    }

    public ICommand AsyncCommand { get; set; }


    public PicketPageViewModel()
    {

        AddCommand = new RelayCommand(AddCommandAction);
        AsyncCommand = new Command(async () => await LoadAction());
        AsyncCommand.Execute(null);
    }

    private void AddCommandAction()
    {
        var profile = ServiceHelper.GetProfile<Teacher>();

        Picket = new Picket() { CreatedBy = profile, CreateAt = DateTime.Now, Date = DateOnly.FromDateTime(DateTime.Now) , };


    }

    [Obsolete]
    private async Task LoadAction()
    {
        try
        {
            var service = ServiceHelper.GetService<IPicketService>();
            Model = await service.GetPicketToday();
            CanSync = true;
            HasPicket = true;
            Message = string.Empty;
        }
        catch (Exception ex)
        {
            Message = ex.Message;
            await Shell.Current.DisplayAlert("Warning", ex.Message, "Ok");
            var scheduleService = ServiceHelper.GetService<IScheduleService>();
            IamPicket = await scheduleService.IamPicket();
        }
    }


}