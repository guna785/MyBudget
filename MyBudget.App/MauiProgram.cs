using CommunityToolkit.Maui.Markup;
using Material.Components.Maui.Extensions;
using Microsoft.Extensions.Logging;

namespace MyBudget.App
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkitMarkup()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });
            builder
          .UseMaterialComponents(new List<string>
          {
                //generally, we needs add 6 types of font families
                "Roboto-Regular.ttf",
                "Roboto-Italic.ttf",
                "Roboto-Medium.ttf",
                "Roboto-MediumItalic.ttf",
                "Roboto-Bold.ttf",
                "Roboto-BoldItalic.ttf",
          });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}