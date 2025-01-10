using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using PicketMobile.Services;
using The49.Maui.BottomSheet;
using ZXing.Net.Maui.Controls;

namespace PicketMobile
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder.Services.AddSingleton<IAccountService, AccountService>();
            builder.Services.AddSingleton<IPicketService, PicketService>();
            builder.Services.AddSingleton<IScheduleService, ScheduleService>();
            builder.Services.AddSingleton<IStudentService, StudentService>();
            builder.UseMauiApp<App>()
                .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            })
                .UseMauiCommunityToolkit()
                 .UseBottomSheet()
                .UseBarcodeReader()
            ;
#if DEBUG
            builder.Logging.AddDebug();
#endif
            return builder.Build();
        }
    }
}