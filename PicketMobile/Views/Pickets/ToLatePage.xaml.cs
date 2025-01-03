using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using PicketMobile.Models;
using PicketMobile.Services;
using SharedModel.Responses;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace PicketMobile.Views.Pickets;

public partial class ToLatePage : ContentPage
{
    public ToLatePage()
    {
        InitializeComponent();
        this.BindingContext = new ToLatePageViewModel();
    }
}

internal class ToLatePageViewModel : BaseNotify
{

    public ObservableCollection<LateAndGoHomeEarlyResponse> DataStudentTolate { get; set; }
    public ICommand AsyncCommand { get; private set; }

    private ICommand addStudentLateCommand;

    public ICommand AddStudentLateCommand
    {
        get { return addStudentLateCommand; }
        set { SetProperty(ref addStudentLateCommand, value); }
    }

    private ICommand addStudentLateByScanCommand;

    public ICommand AddStudentLateByScanCommand
    {
        get { return addStudentLateByScanCommand; }
        set { SetProperty(ref addStudentLateByScanCommand , value); }
    }


    public ICommand SelectBrowseStudent { get; set; }

    public ToLatePageViewModel()
    {

        WeakReferenceMessenger.Default.Register<ToLateChangeMessage>(this, (r, m) =>
        {
            DataStudentTolate.Add(m.Value);
        });
        AsyncCommand = new Command(async () => await LoadAction());
        AddStudentLateByScanCommand = new AsyncRelayCommand(AddStudentLateCommandByScanAction);
        AddStudentLateCommand = new AsyncRelayCommand(AddStudentLateCommandAction);
        DataStudentTolate = new ObservableCollection<LateAndGoHomeEarlyResponse>();
        IsBusy = true;
    }

    private async Task AddStudentLateCommandByScanAction()
    {
        var form = new ScanBarcodePage();
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
        var form = new AddTerlambatPage();
        await Shell.Current.Navigation.PushModalAsync(form);
        //var vm = form.BindingContext as AddTerlambatPageViewModel;
        //if (vm.Model.Id >= 0)
        //{
        //    DataStudentTolate.Add(vm.Model);
        //}
    }
    private async Task LoadAction()
    {
        try
        {
            var service = ServiceHelper.GetService<IPicketService>();
            var picket = await service.GetPicketToday();
            if (picket != null)
            {
                DataStudentTolate.Clear();
                foreach (var item in picket.StudentsLateAndComeHomeEarly.Where(x => x.LateAndGoHomeEarlyStatus == SharedModel.LateAndGoHomeEarlyAttendanceStatus.Terlambat))
                {
                    DataStudentTolate.Add(item);
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