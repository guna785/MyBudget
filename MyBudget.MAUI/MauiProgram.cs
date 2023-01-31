using MyBudget.MAUI.Data;
using MyBudget.MAUI.Extensions;
using Plugin.Fingerprint;
using Plugin.Fingerprint.Abstractions;

namespace MyBudget.MAUI
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            MauiAppBuilder builder = MauiApp.CreateBuilder();

            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                }).AddClientServices();

            builder.Services.AddMauiBlazorWebView();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
#endif

            builder.Services.AddSingleton<WeatherForecastService>();
            builder.Services.AddSingleton<MainPage>();
            builder.Services.AddSingleton(typeof(IFingerprint), CrossFingerprint.Current);

            return builder.Build();
        }
    }
}