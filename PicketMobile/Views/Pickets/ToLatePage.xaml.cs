using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using PicketMobile.Models;
using PicketMobile.Services;
using PicketMobile.Views.BottomSheets;
using SharedModel;
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

public partial class ToLatePageViewModel : BaseNotify
{

    [ObservableProperty]
    private bool hasItems;

    [ObservableProperty]
    private string message = "";

    public ObservableCollection<LateAndGoHomeEarlyResponse> DataStudentTolate { get; set; }
    public ICommand AsyncCommand { get; private set; }

    private ICommand addStudentLateCommand;

    public ICommand AddStudentLateCommand
    {
        get { return addStudentLateCommand; }
        set { SetProperty(ref addStudentLateCommand, value); }
    }




    public ICommand SelectBrowseStudent { get; set; }

    public ToLatePageViewModel()
    {

        WeakReferenceMessenger.Default.Register<ToLateChangeMessage>(this, (r, m) =>
        {
            DataStudentTolate.Add(m.Value);
            HasItems = DataStudentTolate.Count > 0;
        });
        AsyncCommand = new Command(async () => await LoadAction());
        AddStudentLateCommand = new AsyncRelayCommand(AddStudentLateCommandAction);
        DataStudentTolate = new ObservableCollection<LateAndGoHomeEarlyResponse>();
        IsBusy = true;
    }


    private async Task AddStudentLateCommandAction()
    {
        var form = new AddLateAndEarlyHomePage(LateAndGoHomeEarlyAttendanceStatus.Terlambat);
        await Shell.Current.Navigation.PushAsync(form);
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

                HasItems = DataStudentTolate.Count > 0;

                if (!HasItems)
                {
                    Message = "Data Siswa Terlambat belum ada!";
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