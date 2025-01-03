using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using PicketMobile.Models;
using PicketMobile.Services;
using SharedModel;
using SharedModel.Requests;
using SharedModel.Responses;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace PicketMobile.Views.Pickets;

public partial class AddTerlambatPage : ContentPage
{
    public AddTerlambatPage()
    {
        InitializeComponent();
        BindingContext = new AddTerlambatPageViewModel(pickerStudent);
    }
}

internal class AddTerlambatPageViewModel : BaseNotify
{
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

    public ICommand ScandCommand
    {
        get { return scandCommand; }
        set { SetProperty(ref scandCommand, value); }
    }

    public RelayCommand<object> CloseCommand { get; private set; }

    private string searchText;

    public string SearchText
    {
        get { return searchText; }
        set { SetProperty(ref searchText, value); }
    }

    [Obsolete]
    public AddTerlambatPageViewModel(Picker pickerStudent)
    {
        Model = new Models.StudentToLateAndHomeEarlyModel { AtTime = DateTime.Now.TimeOfDay };
        picker = pickerStudent;
        AddCommand = new AsyncRelayCommand<object>(AddAcommandAcation, AddCommandValidate);
        SearchCommand = new RelayCommand<object>(async (x) => await SearchCommandAcation(x), SearchCommandValidate);
        ScandCommand = new RelayCommand<object>(ScanCommandAcation);
        CloseCommand = new RelayCommand<object>(CloseAction);
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
    }

    private void CloseAction(object? obj)
    {
        Shell.Current.CurrentPage.SendBackButtonPressed();
    }

    private void ScanCommandAcation(object? obj)
    {
        throw new NotImplementedException();
    }

    private bool SearchCommandValidate(object? obj)
    {
        return !string.IsNullOrEmpty(SearchText);
    }

    private async Task SearchCommandAcation(object? obj)
    {
        try
        {
            IsBusy = true;
            var studentService = ServiceHelper.GetService<IStudentService>();
            var students = await studentService.SearchStudent(SearchText);
            Students.Clear();
            if (students.Any())
            {
                foreach (var item in students)
                {
                    Students.Add(item);
                }
                picker.Focus();
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
            var result = await picketService.AddLateandEarly(new StudentToLateAndEarlyRequest(Model.Student.Id,
                Model.AtTime,
                Model.Description,
                AttendanceStatus.Hadir,
                SharedModel.LateAndGoHomeEarlyAttendanceStatus.Terlambat));
            if (result != null)
            {
                WeakReferenceMessenger.Default.Send(new ToLateChangeMessage(result));
                await Shell.Current.DisplayAlert("Success", $"{Model.Student.Name} berhasil ditambahkan dalam daftar terlamabat", "OK");
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