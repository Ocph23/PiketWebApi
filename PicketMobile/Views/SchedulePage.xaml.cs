using CommunityToolkit.Mvvm.Input;
using PicketMobile.Models;
using PicketMobile.Services;
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
        RefreshCommand = new AsyncRelayCommand(async (x) => await RefreshCommandAction(x));
        IsBusy = true;
    }

    private async Task RefreshCommandAction(object obj)
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

    [Obsolete]
    private async Task RefreshCommandActionx()
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