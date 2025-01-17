
using CommunityToolkit.Mvvm.ComponentModel;
using PicketMobile.Models;
using PicketMobile.Services;
using SharedModel;
using SharedModel.Responses;
using System.Collections.ObjectModel;

namespace PicketMobile.Views;

public partial class PicketDetailPage : TabbedPage
{
    public PicketDetailPage(int id)
    {
        InitializeComponent();
        BindingContext = new PicketDetailPageViewModel(id);
    }
}

public partial class PicketDetailPageViewModel : BaseNotify
{

    [ObservableProperty]
    private ObservableCollection<LateAndGoHomeEarlyResponse> lateSource = new ObservableCollection<LateAndGoHomeEarlyResponse>();

    [ObservableProperty]
    private ObservableCollection<LateAndGoHomeEarlyResponse> goHomeSource = new ObservableCollection<LateAndGoHomeEarlyResponse>();

    private int id;

    public PicketDetailPageViewModel(int id)
    {
        this.id = id;
        _ = Load();
    }

    [ObservableProperty]
    public PicketModel? model = new PicketModel();

    private async Task Load()
    {
        try
        {
            IsBusy = true;
            var service = ServiceHelper.GetService<IPicketService>();
            var result = await service.GetById(id);
            if (result != null)
            {
                Model.CreateAt = result.CreateAt;
                Model.Date = result.Date;
                Model.EndAt = result.EndAt;
                Model.Id = result.Id;
                Model.StartAt = result.StartAt;
                Model.Weather = result.Weather;
                Model.CreatedBy = new TeacherResponse { Name = result.CreatedName, RegisterNumber = result.CreatedNumber };
                Model.TeacherAttendance = result.TeacherAttendance;
                LateSource.Clear();
                GoHomeSource.Clear();

                foreach (var item in result.StudentsLateAndComeHomeEarly)
                {
                    if (item.LateAndGoHomeEarlyStatus == LateAndGoHomeEarlyAttendanceStatus.Terlambat)
                        LateSource.Add(item);
                    else
                        GoHomeSource.Add(item);
                }
            }
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