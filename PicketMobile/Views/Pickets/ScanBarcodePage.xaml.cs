using ZXing;
using ZXing.Common;
using ZXing.Net.Maui;

namespace PicketMobile.Views.Pickets;

public partial class ScanBarcodePage : ContentPage
{

   
    public ScanBarcodePage()
    {
        InitializeComponent();
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
