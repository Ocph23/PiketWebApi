using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PicketMobile.Services;
using SharedModel.Responses;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace PicketMobile.Views;

public partial class HistoryPage : ContentPage
{
    public HistoryPage()
    {
        InitializeComponent();
        BindingContext = new HistoryPageViewModel();
    }
}

public partial class HistoryPageViewModel : BaseNotify
{

    [ObservableProperty]
    int remainingThreshold = -1;

    [ObservableProperty]
    ObservableCollection<PicketResponse> datas = new ObservableCollection<PicketResponse>();

    [ObservableProperty]
    private int currentPage=1;

    [ObservableProperty]
    private int pageSize=6;

    [ObservableProperty]
    private string? orderColumn= "date";

    [ObservableProperty]
    private ICommand loadDataCommand;

    [ObservableProperty]
    private PicketResponse selectedItem;

    [ObservableProperty]
    private ICommand selectItemCommand;

    [ObservableProperty]
    private ICommand loadMoreDataCommand;

    private bool hasNextItems;

    [ObservableProperty]
    private bool isBusyDataMore;



    public HistoryPageViewModel()
    {

        SelectItemCommand = new AsyncRelayCommand(async (item) =>
        {
            await Shell.Current.Navigation.PushModalAsync(new PicketDetailPage(SelectedItem.Id));
        });

        LoadDataCommand = new AsyncRelayCommand(LoadData);
        LoadMoreDataCommand = new AsyncRelayCommand(LoadMoreData);
        IsBusy = true;
        LoadDataCommand.Execute(null);
    }

    public async Task LoadData()
    {
        try
        {
            IsBusy= true;
            IsBusyDataMore = true;
            await Task.Delay(500);

            CurrentPage = 1;
            Datas.Clear();
            var service = ServiceHelper.GetService<IPicketService>();
            var result = await service.Get(new SharedModel.Requests.PaginationRequest(CurrentPage, PageSize, "", "desc", OrderColumn!));
            if (result != null)
            {
                foreach (var item in result.Data)
                {
                    Datas.Add(item);
                }
            }

            hasNextItems = CurrentPage < result.TotalRecords / PageSize;
            if (hasNextItems)
            {
                RemainingThreshold = 5;
            }

        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", ex.Message, "Ok");
        }
        finally
        {
            IsBusy= false;
            IsBusyDataMore = false;
        }
    }

    public async Task LoadMoreData()
    {
        try
        {
            IsBusyDataMore = true;
            CurrentPage++;
            var service = ServiceHelper.GetService<IPicketService>();
            var result = await service.Get(new SharedModel.Requests.PaginationRequest(CurrentPage, PageSize, "", "desc", ""));
            if (result != null)
            {
                foreach (var item in result.Data)
                {
                    Datas.Add(item);
                }
            }
            hasNextItems = CurrentPage < Math.Ceiling(Convert.ToDouble(result.TotalRecords / PageSize));
            RemainingThreshold = hasNextItems ? 5 : -1;
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", ex.Message, "Ok");
        }
        finally
        {
            IsBusyDataMore = false;
        }
    }
}