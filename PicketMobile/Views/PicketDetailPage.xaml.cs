
using CommunityToolkit.Mvvm.ComponentModel;
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
    public PicketResponse? model;

    private async Task Load()
    {
        try
        {
            var service = ServiceHelper.GetService<IPicketService>();
            Model = await service.GetById(id);
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", ex.Message, "Keluar");
        }
    }
}