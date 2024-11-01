using System.Collections.ObjectModel;

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

    public ObservableCollection<SharedModel.Models.StudentToLate> DataStudentTolate { get; set; }


    public ToLatePageViewModel()
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