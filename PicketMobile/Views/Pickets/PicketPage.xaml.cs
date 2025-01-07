using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PicketMobile.Models;
using PicketMobile.Services;
using SharedModel;
using SharedModel.Responses;
using System.Collections.ObjectModel;
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
        UpdateCommand = new AsyncRelayCommand(UpdateCommandAction, UpdateCommandValidate);
        AsyncCommand = new Command(async () => await LoadAction());
        AsyncCommand.Execute(null);
    }

    private bool UpdateCommandValidate()
    {
        if (Model == null || Model.StartAt == null || Model.EndAt == null)
            return false;
        return true;
    }

    private async Task UpdateCommandAction()
    {
        try
        {
            var profile = ServiceHelper.GetProfile<TeacherResponse>();
            var picketService = ServiceHelper.GetService<IPicketService>();
            this.Model.CreatedBy = profile;
            this.Model.CreateAt = DateTime.Now.ToUniversalTime();
            var requestModel = new PicketRequest
            {
                CreateAt = Model.CreateAt,
                CreatedId = Model.CreatedBy.Id,
                Date = Model.Date,
                EndAt = Model.EndAt,
                StartAt = Model.StartAt,
                Weather = Model.Weather,
                Id = Model.Id
            };
            var result = await picketService.Put(Model.Id, requestModel);
            if (result)
            {
                IsChange = false;
                await Shell.Current.DisplayAlert("Success", "Data Piket berhasil diupdate", "Keluar");
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
            var profile = ServiceHelper.GetProfile<TeacherResponse>();
            var picketService = ServiceHelper.GetService<IPicketService>();
            var model = new PicketModel() { CreateAt = DateTime.Now.ToUniversalTime(), Date = DateOnly.FromDateTime(DateTime.Now), };
            model.CreatedBy = profile;
            var result = await picketService.Create(model);
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


    private async Task LoadAction()
    {
        try
        {
            Message= "Tunggu ......!";
            IamPicket = false;
            var service = ServiceHelper.GetService<IPicketService>();
            var response = await service.GetPicketToday();
            Model = new PicketModel
            {
                CreateAt = response.CreateAt,
                Date = response.Date,
                CreatedBy = new TeacherResponse { Id = response.CreatedId, Name = response.CreatedName },
                EndAt = response.EndAt,
                Id = response.Id,
                StartAt = response.StartAt,
                Weather = response.Weather,
            };
            if (Model != null)
            {
                Model.PropertyChanged += (s, p) =>
                {
                    IsChange = true;
                };
                HasPicket = true;
                Message = string.Empty;
            }

        }
        catch (Exception ex)
        {
            if (Shell.Current != null)
            {
                await Shell.Current.DisplayAlert("Warning", ex.Message, "Ok");
            }
            Message = ex.Message;

            await Task.Delay(1000);

            if (Message.ToLower().Contains("Piket Belum Di buka".ToLower()))
            {
                var scheduleService = ServiceHelper.GetService<IScheduleService>();
                IamPicket = await scheduleService.IamPicket();
            }

        }
        finally
        {
            CanSync = false;
        }
    }


}