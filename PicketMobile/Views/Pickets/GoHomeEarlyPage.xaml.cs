using SharedModel.Models;
using System.Collections.ObjectModel;

namespace PicketMobile.Views.Pickets;

public partial class GoHomeEarlyPage : ContentPage
{
    public GoHomeEarlyPage()
    {
        InitializeComponent();
        BindingContext = new GoHomeEarlyPageViewModel();
    }
}

internal class GoHomeEarlyPageViewModel : BaseNotify
{

    public ObservableCollection<StudentComeHomeEarly> DataStudent { get; set; }


    public GoHomeEarlyPageViewModel()
    {
        DataStudent = new ObservableCollection<StudentComeHomeEarly>();
        DataStudent.Add(new StudentComeHomeEarly()
        {
            Student = new Student { Name = "Avip Siapa Saja" },

        });
        DataStudent.Add(new StudentComeHomeEarly()
        {
            Student = new Student { Name = "Ismail Mana Saja Juga" },

        });

    }


}