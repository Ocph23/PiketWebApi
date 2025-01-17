
using CommunityToolkit.Mvvm.ComponentModel;
using PicketMobile.Models;
using PicketMobile.Services;
using SharedModel.Responses;

namespace PicketMobile.Views;

public partial class PicketDetailPage : ContentPage
{
    public PicketDetailPage(int id)
    {
        InitializeComponent();
        BindingContext = new PicketDetailPageViewModel(id);
    }
}

public partial class PicketDetailPageViewModel : BaseNotify
{
    private int id;

    public PicketDetailPageViewModel(int id)
    {
        this.id = id;
        _ = Load();
    }

    [ObservableProperty]
    public PicketModel? model;

    private async Task Load()
    {
        try
        {
            IsBusy = true;
            var service = ServiceHelper.GetService<IPicketService>();
            var result  = await service.GetById(id);
            Model = new PicketModel {
                CreateAt = result.CreateAt,
                Date = result.Date,
                EndAt = result.EndAt,
                Id = result.Id,
                StartAt = result.StartAt,
                Weather = result.Weather,
                CreatedBy=new TeacherResponse { Name=result.CreatedName, RegisterNumber = result.CreatedNumber },
                LateAndComeHomeEarly = result.StudentsLateAndComeHomeEarly,
            };
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", ex.Message, "Keluar");
        }
        finally
        {
            IsBusy = false;
        }
    }
}