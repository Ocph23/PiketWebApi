using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.Messaging;
using PicketMobile.Models;
using PicketMobile.Services;
using SharedModel;
using SharedModel.Requests;
using SharedModel.Responses;
using System.Text.Json;
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


    private async void cameraBarcodeReaderView_BarcodesDetected(object sender, ZXing.Net.Maui.BarcodeDetectionEventArgs e)
    {
        IsDetecting = false;
        var first = e.Results?.FirstOrDefault();
        if (first is null || first.Value == LastScan)
        {
            IsDetecting = true;
            return;
        }
        LastScan = first.Value;


        var studentService = ServiceHelper.GetService<IStudentService>();
        var students = await studentService.SearchStudent(LastScan);
        if (students.Any())
        {
            Dispatcher.DispatchAsync(async () =>
            {
                WeakReferenceMessenger.Default.Send(new StudentSearchChangeMessage(students.First()));
                await Task.Delay(1000);
                await Shell.Current.Navigation.PopModalAsync();

            });
        }
        else
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            ToastDuration duration = ToastDuration.Short;
            double fontSize = 14;
            var toast = Toast.Make("Data tidak ditemukan !", duration, fontSize);
            await toast.Show(cancellationTokenSource.Token);
            IsDetecting = true;
            LastScan = string.Empty;
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

    private void Button_Clicked(object sender, EventArgs e)
    {
        //var dataStudent = @"{
        //                    ""nis"": ""3066247132"",
        //                    ""nisn"": """",
        //                    ""id"": 1,
        //                    ""gender"": 0,
        //                    ""name"": ""Aldrich"",
        //                    ""placeOfBorn"": ""Makassar"",
        //                    ""dateOfBorn"": ""2016-02-25"",
        //                    ""email"": """",
        //                    ""description"": """",
        //                    ""photo"": """",
        //                    ""userId"": """"
        //                  }";

        //var student = JsonSerializer.Deserialize<StudentResponse>(dataStudent, Helper.JsonOption);
        //WeakReferenceMessenger.Default.Send(new StudentSearchChangeMessage(student));

        Shell.Current.Navigation.PopModalAsync();


    }
}
