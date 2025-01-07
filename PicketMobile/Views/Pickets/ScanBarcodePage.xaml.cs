using CommunityToolkit.Mvvm.Messaging;
using PicketMobile.Models;
using PicketMobile.Services;
using SharedModel;
using SharedModel.Requests;
using SharedModel.Responses;
using ZXing;
using ZXing.Common;
using ZXing.Net.Maui;

namespace PicketMobile.Views.Pickets;

public partial class ScanBarcodePage : ContentPage
{
    LateAndGoHomeEarlyAttendanceStatus lateAndGoHomeEarlyAttendanceStatus;

    public ScanBarcodePage(LateAndGoHomeEarlyAttendanceStatus status)
    {
        InitializeComponent();
        lateAndGoHomeEarlyAttendanceStatus = status;
        BindingContext = this;
        cameraBarcodeReaderView.Options = new BarcodeReaderOptions
        {
            Formats = BarcodeFormats.TwoDimensional,
            AutoRotate = true,
            Multiple = true
        };
    }


    private void cameraBarcodeReaderView_BarcodesDetected(object sender, ZXing.Net.Maui.BarcodeDetectionEventArgs e)
    {

        IsDetecting = false;
        var first = e.Results?.FirstOrDefault();
        if (first is null || first.Value == LastScan)
        {
            IsDetecting = true;
            return;
        }
        LastScan = first.Value;
        Dispatcher.DispatchAsync(async () =>
        {
            await DisplayAlert("Barcode Detected", first.Value, "OK");
            await Task.Delay(1000);







            IsDetecting = true;
            LastScan = string.Empty;
        });
    }

    [Obsolete]
    public async Task AddStudentToLateOrGoHomeEarly(StudentResponse student)
    {
        try
        {
            IsBusy = true;
            var picketService = ServiceHelper.GetService<IPicketService>();
            var result = await picketService.AddLateandEarly(
                new StudentToLateAndEarlyRequest(student.Id,
                DateTime.Now.TimeOfDay,
                "Model.Description",
                AttendanceStatus.Hadir,
                lateAndGoHomeEarlyAttendanceStatus));


            if (result != null)
            {
                if (lateAndGoHomeEarlyAttendanceStatus == LateAndGoHomeEarlyAttendanceStatus.Terlambat)
                {
                    WeakReferenceMessenger.Default.Send(new ToLateChangeMessage(result));
                    await Shell.Current.DisplayAlert("Success", $"{student.Name} berhasil ditambahkan dalam daftar terlamabat", "OK");
                }
                else
                {
                    WeakReferenceMessenger.Default.Send(new ToEarlyGoHomeChangeMessage(result));
                    await Shell.Current.DisplayAlert("Success", $"{student.Name} berhasil ditambahkan dalam daftar pulang", "OK");
                }
            }
            IsBusy = false;
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }



    private bool isDetecting = true;

    public bool IsDetecting
    {
        get { return isDetecting; }
        set
        {
            isDetecting = value;
            OnPropertyChanging("IsDetecting");
        }
    }


    private string lastScan;

    public string LastScan
    {
        get { return lastScan; }
        set
        {
            lastScan = value;
            OnPropertyChanging("LastScan");
        }
    }

    private void BarcodesDetected(object sender, BarcodeDetectionEventArgs e)
    {

    }
}
