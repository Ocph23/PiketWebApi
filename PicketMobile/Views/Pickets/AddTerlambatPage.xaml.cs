using CommunityToolkit.Mvvm.Input;
using PicketMobile.Services;
using SharedModel.Models;
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
    private Models.StudentToLateModel studentToLateModel = new Models.StudentToLateModel();

    public ObservableCollection<Student> Students { get; set; } = new ObservableCollection<Student>();

    public Models.StudentToLateModel Model
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



    private string searchText;

    public string SearchText
    {
        get { return searchText; }
        set { SetProperty(ref searchText, value); }
    }


    public AddTerlambatPageViewModel(Picker pickerStudent)
    {
        Model = new Models.StudentToLateModel { AtTime = DateTime.Now.TimeOfDay };
        picker = pickerStudent;
        AddCommand = new AsyncRelayCommand<object>(AddAcommandAcation, AddCommandValidate);
        SearchCommand = new RelayCommand<object>(async (x) => await SearchCommandAcation(x), SearchCommandValidate);
        ScandCommand = new RelayCommand<object>(ScanCommandAcation);

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
    }

    [Obsolete]
    private async Task AddAcommandAcation(object? obj)
    {
        try
        {
            IsBusy = true;
            var picketService = ServiceHelper.GetService<IPicketService>();
            var result = await picketService.PostToLate(Model);
            if (result != null)
            {
                Model.Id = result.Id;
                await Shell.Current.DisplayAlert("Success", $"{Model.Student.Name} berhasil ditambahkan dalam daftar terlamabat", "OK");
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