using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PicketMobile.Services
{
    public class ServiceHelper
    {
        [Obsolete]
        public static TService GetService<TService>() => Current.GetService<TService>()!;

        internal static T GetProfile<T>()
        {
            var profile = Preferences.Get("profile", null);
            if (profile == null)
                return default(T);
            return JsonSerializer.Deserialize<T>(profile, Helper.JsonOption);
        }

        [Obsolete]
        public static IServiceProvider Current =>
#if WINDOWS10_0_17763_0_OR_GREATER
        MauiWinUIApplication.Current.Services;
#elif __ANDROID__
        MauiApplication.Current.Services;

#elif IOS || MACCATALYST
        MauiUIApplicationDelegate.Current.Services;
#else
        null;
#endif
    }
}
