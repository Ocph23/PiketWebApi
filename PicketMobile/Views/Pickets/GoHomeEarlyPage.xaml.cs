using CommunityToolkit.Mvvm.Input;
using SharedModel.Responses;
using System.Collections.ObjectModel;
using System.Windows.Input;
using PicketMobile.Services;
using CommunityToolkit.Mvvm.Messaging;
using PicketMobile.Models;
using SharedModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace PicketMobile.Views.Pickets;

public partial class GoHomeEarlyPage : ContentPage
{
    public GoHomeEarlyPage()
    {
        InitializeComponent();
        this.BindingContext = new GoHomeEarlyPageViewModel();
    }
}

public partial class GoHomeEarlyPageViewModel : BaseNotify
{

    [ObservableProperty]
    bool hasItems;


    [ObservableProperty]
    string message="";

    public ObservableCollection<LateAndGoHomeEarlyResponse> DataStudentToEarly { get; set; } = new ObservableCollection<LateAndGoHomeEarlyResponse>();
    public ICommand? AsyncCommand { get; private set; }

    private ICommand? addStudentLateCommand;

    public ICommand? AddStudentLateCommand
    {
        get { return addStudentLateCommand; }
        set { SetProperty(ref addStudentLateCommand, value); }
    }

    private ICommand? addStudentLateByScanCommand;

    public ICommand? AddStudentLateByScanCommand
    {
        get { return addStudentLateByScanCommand; }
        set { SetProperty(ref addStudentLateByScanCommand, value); }
    }


    public ICommand? SelectBrowseStudent { get; set; }

    public GoHomeEarlyPageViewModel()
    {
        WeakReferenceMessenger.Default.Register<ToEarlyGoHomeChangeMessage>(this, (r, m) =>
        {
            DataStudentToEarly.Add(m.Value);
            HasItems = DataStudentToEarly.Count > 0;
        });
        AsyncCommand = new Command(async () => await LoadAction());
        AddStudentLateByScanCommand = new AsyncRelayCommand(AddStudentLateCommandByScanAction);
        AddStudentLateCommand = new AsyncRelayCommand(AddStudentLateCommandAction);
        IsBusy = true;
    }

    private async Task AddStudentLateCommandByScanAction()
    {
        var form = new ScanBarcodePage(LateAndGoHomeEarlyAttendanceStatus.Pulang);
        await Shell.Current.Navigation.PushModalAsync(form);
    }

    [Obsolete]
    private async Task<bool> AddStudentLateCommandValidation()
    {
        var scheduleService = ServiceHelper.GetService<IScheduleService>();
        return await scheduleService.IamPicket();
    }

    private async Task AddStudentLateCommandAction()
    {
        var form = new AddLateAndEarlyHomePage(LateAndGoHomeEarlyAttendanceStatus.Pulang);
        await Shell.Current.Navigation.PushModalAsync(form);
    }

    private async Task LoadAction()
    {
        try
        {
            var service = ServiceHelper.GetService<IPicketService>();
            var picket = await service.GetPicketToday();
            if (picket != null)
            {
                DataStudentToEarly.Clear();
                foreach (var item in picket.StudentsLateAndComeHomeEarly.Where(x => x.LateAndGoHomeEarlyStatus == SharedModel.LateAndGoHomeEarlyAttendanceStatus.Pulang))
                {
                    DataStudentToEarly.Add(item);
                }

                HasItems = DataStudentToEarly.Count > 0;

                if (!HasItems)
                {
                    Message = "Data siswa pulang lebih cepat belum ada!";
                }
                else
                {
                    Message = string.Empty;
                }

            }
        }
        catch (Exception ex)
        {
            if (Shell.Current != null)
            {
                await Shell.Current.DisplayAlert("Warning", ex.Message, "Ok");
                Message = ex.Message;
            }
        }
        finally
        {
            IsBusy = false;
        }
    }


}