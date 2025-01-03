using CommunityToolkit.Mvvm.Input;
using SharedModel.Responses;
using System.Collections.ObjectModel;
using System.Windows.Input;
using PicketMobile.Services;
using CommunityToolkit.Mvvm.Messaging;
using PicketMobile.Models;

namespace PicketMobile.Views.Pickets;

public partial class GoHomeEarlyPage : ContentPage
{
    public GoHomeEarlyPage()
    {
        InitializeComponent();
        this.BindingContext = new GoHomeEarlyPageViewModel();
    }
}

internal partial class GoHomeEarlyPageViewModel : BaseNotify
{

    public ObservableCollection<LateAndGoHomeEarlyResponse> DataStudentGoHomeEarly { get; set; }

  

    public ICommand AsyncCommand { get; private set; }

    private ICommand addStudentLateCommand;

    public ICommand AddStudentLateCommand
    {
        get { return addStudentLateCommand; }
        set { SetProperty(ref addStudentLateCommand, value); }
    }

    public ICommand SelectBrowseStudent { get; set; }

    public GoHomeEarlyPageViewModel()
    {
        AsyncCommand = new Command(async () => await LoadAction());
        AddStudentLateCommand = new AsyncRelayCommand(AddStudentLateCommandAction);
        DataStudentGoHomeEarly = new ObservableCollection<LateAndGoHomeEarlyResponse>();
        WeakReferenceMessenger.Default.Register<ToEarlyGoHomeChangeMessage>(this, (r, m) =>
        {
            DataStudentGoHomeEarly.Add(m.Value);
        });
        IsBusy = true;
    }

    [Obsolete]
    private async Task<bool> AddStudentLateCommandValidation()
    {
        var scheduleService = ServiceHelper.GetService<IScheduleService>();
        return await scheduleService.IamPicket();
    }

    private async Task AddStudentLateCommandAction()
    {
        var form = new AddGoHomeEarlyPage();
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
                DataStudentGoHomeEarly.Clear();
                foreach (var item in picket.StudentsLateAndComeHomeEarly.Where(x=>x.LateAndGoHomeEarlyStatus== SharedModel.LateAndGoHomeEarlyAttendanceStatus.Pulang))
                {
                    DataStudentGoHomeEarly.Add(item);
                }
            }
        }
        catch (Exception ex)
        {
            if (Shell.Current != null)
            {
                await Shell.Current.DisplayAlert("Warning", ex.Message, "Ok");
            }
        }
        finally
        {
            IsBusy = false;
        }
    }



}