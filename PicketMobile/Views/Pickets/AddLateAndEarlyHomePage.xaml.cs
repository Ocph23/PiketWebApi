using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using PicketMobile.Models;
using PicketMobile.Services;
using PicketMobile.Views.BottomSheets;
using SharedModel;
using SharedModel.Requests;
using SharedModel.Responses;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace PicketMobile.Views.Pickets;

public partial class AddLateAndEarlyHomePage : ContentPage
{
    public AddLateAndEarlyHomePage(LateAndGoHomeEarlyAttendanceStatus lateAndGoHomeStatus)
    {
        InitializeComponent();
        BindingContext = new AddTerlambatPageViewModel(lateAndGoHomeStatus);
    }
}

public partial class AddTerlambatPageViewModel : BaseNotify
{

    [ObservableProperty]
    private LateAndGoHomeEarlyAttendanceStatus lateAndGoHomeEarlyStatus;


    private Models.StudentToLateAndHomeEarlyModel studentToLateModel = new Models.StudentToLateAndHomeEarlyModel();

    public ObservableCollection<StudentResponse> Students { get; set; } = new ObservableCollection<StudentResponse>();

    public Models.StudentToLateAndHomeEarlyModel Model
    {
        get { return studentToLateModel; }
        set { SetProperty(ref studentToLateModel, value); }
    }


    private ICommand addCommand;
    private Picker picker;

    public ICommand AddCommand
    {
        get { return addCommand; }
        set { SetProperty(ref addCommand, value); }
    }

    private ICommand searchCommand;

    public ICommand SearchCommand
    {
        get { return searchCommand; }
        set { SetProperty(ref searchCommand, value); }
    }


    private ICommand scandCommand;

    public ICommand ScanCommand
    {
        get { return scandCommand; }
        set { SetProperty(ref scandCommand, value); }
    }

    public ICommand CloseCommand { get;  set; }

    private string searchText;

    public string SearchText
    {
        get { return searchText; }
        set { SetProperty(ref searchText, value); }
    }

    [Obsolete]
    public AddTerlambatPageViewModel(LateAndGoHomeEarlyAttendanceStatus lateAndGoHomeStatus)
    {

        Title = lateAndGoHomeStatus == LateAndGoHomeEarlyAttendanceStatus.Terlambat ? 
            "Tambah Siswa Terlambat" : "Tambah Siswa Pulang Lebih Cepat";


        LateAndGoHomeEarlyStatus = lateAndGoHomeStatus;
        Model = new Models.StudentToLateAndHomeEarlyModel { AtTime = DateTime.Now.TimeOfDay };
        AddCommand = new AsyncRelayCommand<object>(AddAcommandAcation, AddCommandValidate);
        SearchCommand = new RelayCommand<object>(async (x) => await SearchCommandAcation(x), SearchCommandValidate);
        ScanCommand = new AsyncRelayCommand(ScanCommandAcation);
        CloseCommand = new AsyncRelayCommand(CloseAction);
        this.PropertyChanged += (s, p) =>
        {
            if (p.PropertyName == "SearchText")
            {
                SearchCommand = new RelayCommand<object>(async (x) => await SearchCommandAcation(x), SearchCommandValidate);
            }
        };


        this.Model.PropertyChanged += (s, p) =>
        {
            if (p.PropertyName == "Student" || p.PropertyName == "Description" || p.PropertyName == "AtTime")
            {
                AddCommand = new AsyncRelayCommand<object>(AddAcommandAcation, AddCommandValidate);
            }

        };

        WeakReferenceMessenger.Default.Register<StudentSearchChangeMessage>(this, (r, m) =>
        {
            if (m.Value != null)
            {
                Model.Student = m.Value;
            }
        });

    }

    private async Task ScanCommandAcation()
    {
        var scannerPage = new ScanBarcodePage(LateAndGoHomeEarlyStatus);
        await Shell.Current.Navigation.PushModalAsync(scannerPage);
    }

    private async Task CloseAction()
    {
       await Shell.Current.Navigation.PopModalAsync();
    }


    private bool SearchCommandValidate(object? obj)
    {
        return !string.IsNullOrEmpty(SearchText);
    }

    [Obsolete]
    private async Task SearchCommandAcation(object? obj)
    {
        try
        {
            IsBusy = true;
            var studentService = ServiceHelper.GetService<IStudentService>();
            var students = await studentService.SearchStudent(SearchText);

            if (students.Any())
            {
                var bs = new BrowseStudentBottomSheet(students);
                await bs.ShowAsync();
            }
            else
            {
                await Shell.Current.DisplayAlert("Info", "Data tidak ditemukan.", "Ok");
            }
        }
        catch (Exception ex)
        {
            if (Shell.Current != null)
            {
                await Shell.Current.DisplayAlert("Error", ex.Message, "Ok");
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    [Obsolete]
    private async Task AddAcommandAcation(object? obj)
    {
        try
        {
            IsBusy = true;
            var picketService = ServiceHelper.GetService<IPicketService>();
            var result = await picketService.AddLateandEarly(
                new StudentToLateAndEarlyRequest(Model.Student.Id,
                Model.AtTime,
                Model.Description,
                AttendanceStatus.Hadir,
                LateAndGoHomeEarlyStatus));


            if (result != null)
            {
                if (LateAndGoHomeEarlyStatus == LateAndGoHomeEarlyAttendanceStatus.Terlambat)
                {
                    WeakReferenceMessenger.Default.Send(new ToLateChangeMessage(result));
                    await Shell.Current.DisplayAlert("Success", $"{Model.Student.Name} berhasil ditambahkan dalam daftar terlamabat", "OK");
                }
                else
                {
                    WeakReferenceMessenger.Default.Send(new ToEarlyGoHomeChangeMessage(result));
                    await Shell.Current.DisplayAlert("Success", $"{Model.Student.Name} berhasil ditambahkan dalam daftar pulang", "OK");
                }


                SearchText = string.Empty;
                Model = new Models.StudentToLateAndHomeEarlyModel();
            }
            IsBusy = false;
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private bool AddCommandValidate(object? obj)
    {
        if (Model.Student == null || (Model.AtTime < new TimeSpan(8, 0, 0)))
        {
            return false;
        }
        return true;
    }
}