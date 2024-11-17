using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PicketMobile.Models;
using PicketMobile.Services;
using SharedModel;
using SharedModel.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
    PicketModel model;//= new PicketModel() { CreateAt = DateTime.Now, Date = DateOnly.FromDateTime(DateTime.Now), };

    [ObservableProperty]
    private bool canSync;

    [ObservableProperty]
    private bool hasPicket;

    [ObservableProperty]
    private bool iamPicket;


    [ObservableProperty]
    ImageSource asyncCommandIcon = ImageSource.FromFile("sync.svg");



    [ObservableProperty]
    private bool isChange;


    [ObservableProperty]
    private string message = "";

    [ObservableProperty]
    ICommand addCommand;

    [ObservableProperty]
    ObservableCollection<Weather> weathers;

    public ICommand AsyncCommand { get; set; }
    public ICommand UpdateCommand { get; set; }




    public PicketPageViewModel()
    {
        Weathers = new ObservableCollection<Weather>(Enum.GetValues(typeof(Weather)).Cast<Weather>());
        AddCommand = new AsyncRelayCommand(AddCommandAction);
        UpdateCommand = new AsyncRelayCommand(UpdateCommandAction);
        AsyncCommand = new Command(async () => await LoadAction());
        CanSync = true;
    }

    private async Task UpdateCommandAction()
    {
        try
        {
            var profile = ServiceHelper.GetProfile<Teacher>();
            var picketService = ServiceHelper.GetService<IPicketService>();
            this.Model.CreatedBy = profile;
            this.Model.CreateAt = DateTime.Now.ToUniversalTime();
            var result = await picketService.Put(Model.Id, Model);
            if (result != null)
            {
                IsChange = false;
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", ex.Message, "Close");
        }
    }

    private async Task AddCommandAction()
    {
        try
        {
            var profile = ServiceHelper.GetProfile<Teacher>();
            var picketService = ServiceHelper.GetService<IPicketService>();
            Model = new PicketModel() { CreateAt = DateTime.Now.ToUniversalTime(), Date = DateOnly.FromDateTime(DateTime.Now), };
            this.Model.CreatedBy = profile;
            var result = await picketService.Create(Model);
            if (result != null)
            {
                Model.Id = result.Id;
                CanSync = true;
                HasPicket = true;
                IamPicket = false;
                IsChange = false;
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", ex.Message, "Close");
        }
    }


    [Obsolete]
    private async Task LoadAction()
    {
        try
        {
            var service = ServiceHelper.GetService<IPicketService>();
            Model = await service.GetPicketToday();
            if (Model != null)
            {
                Model.PropertyChanged += (s, p) =>
                {
                    IsChange = true;
                };
            }

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
        finally
        {
            CanSync = false;
        }
    }


}