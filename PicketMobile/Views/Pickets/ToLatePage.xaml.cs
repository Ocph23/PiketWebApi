using CommunityToolkit.Mvvm.Input;
using PicketMobile.Models;
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

    public ObservableCollection<StudentToLateModel> DataStudentTolate { get; set; }

    public ICommand AddStudentLateCommand { get; set; }

    public ToLatePageViewModel()
    {
        AddStudentLateCommand = new RelayCommand(AddStudentLateCommandAction);
        DataStudentTolate = new ObservableCollection<StudentToLateModel>();
        DataStudentTolate.Add(new StudentToLateModel()
        {
            Student = new SharedModel.Models.Student { Name = "Avip Siapa Saja" },

        });
        DataStudentTolate.Add(new StudentToLateModel()
        {
            Student = new SharedModel.Models.Student { Name = "Ismail Mana Saja Juga" },

        });

    }

    private void AddStudentLateCommandAction()
    {
        Shell.Current.Navigation.PushModalAsync(new AddTerlambatPage());
    }
}