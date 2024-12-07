using CommunityToolkit.Mvvm.Input;
using PicketMobile.Models;
using PicketMobile.Services;
using SharedModel.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace PicketMobile.Views;

public partial class SchedulePage : ContentPage
{
    [Obsolete]
    public SchedulePage()
    {
        InitializeComponent();
        BindingContext = new SchedulePageViewModel();
    }
}

internal class SchedulePageViewModel : BaseNotify
{

    public ICommand RefreshCommand { get; set; }


    public ObservableCollection<ScheduleModel> Datas { get; set; } = new ObservableCollection<ScheduleModel>();

    [Obsolete]
    public SchedulePageViewModel()
    {
        RefreshCommand = new RelayCommand(async () => await RefreshCommandAction());
        IsBusy = true;
    }

    [Obsolete]
    private async Task RefreshCommandAction()
    {
        try
        {
            var service = ServiceHelper.GetService<IScheduleService>();
            var data = await service.GetScheduleActive();
            Datas.Clear();
            foreach (var item in data)
            {
                Datas.Add(item);
            }
            IsBusy = false;
        }
        catch (Exception ex)
        {
            await AppShell.Current.DisplayAlert("Error", ex.Message, "Ok");
        }
        finally { IsBusy = false; }
    }


}