using CommunityToolkit.Mvvm.Input;
using PicketMobile.Models;
using PicketMobile.Services;
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
    public ICommand AsyncCommand { get; private set; }
    public ICommand AddStudentLateCommand { get; set; }
    public ICommand SelectBrowseStudent { get; set; }   

    public ToLatePageViewModel()
    {
        AsyncCommand = new Command(async () => await LoadAction());
        AddStudentLateCommand = new AsyncRelayCommand(AddStudentLateCommandAction);
        DataStudentTolate = new ObservableCollection<StudentToLateModel>();

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
                foreach (var item in picket.StudentsToLate)
                {
                    DataStudentTolate.Add(new StudentToLateModel() { Student=item.Student,
                     CreateAt=item.CreateAt, CreatedBy=item.CreatedBy, Description=item.Description, Id=item.Id,
                     });
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