using System.Collections.ObjectModel;

namespace PicketMobile.Views.Pickets;

public partial class GoHomeEarlyPage : ContentPage
{
    public GoHomeEarlyPage()
    {
        InitializeComponent();
        this.BindingContext = new GoHomeEarlyPageViewModel();
    }
}

internal class GoHomeEarlyPageViewModel : BaseNotify
{

    public ObservableCollection<SharedModel.Models.StudentToLate> DataStudentTolate { get; set; }


    public GoHomeEarlyPageViewModel()
    {
        DataStudentTolate = new ObservableCollection<SharedModel.Models.StudentToLate>();
        DataStudentTolate.Add(new SharedModel.Models.StudentToLate()
        {
            Student = new SharedModel.Models.Student { Name = "Avip Siapa Saja" },

        });
        DataStudentTolate.Add(new SharedModel.Models.StudentToLate()
        {
            Student = new SharedModel.Models.Student { Name = "Ismail Mana Saja Juga" },

        });

    }
}