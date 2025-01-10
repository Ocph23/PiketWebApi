using The49.Maui.BottomSheet;

namespace PicketMobile.Views.BottomSheets;

public partial class ScanStudentBottmSheet : BottomSheet
{


    public ScanStudentBottmSheet()
    {
        InitializeComponent();
        BindingContext = this;
    }




    private void Button_Clicked(object sender, EventArgs e)
    {
        this.DismissAsync();
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
            await Shell.Current.DisplayAlert("Barcode Detected", first.Value, "OK");
            await Task.Delay(1000);
            IsDetecting = true;
            LastScan = string.Empty;
        });
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
}