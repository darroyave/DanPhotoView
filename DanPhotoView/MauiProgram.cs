using CommunityToolkit.Maui;
using DanPhotoView.Controls;
using DanPhotoView.ViewModels;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace DanPhotoView
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif
            builder.Services.TryAddTransient<MainPage>();

            builder.Services.AddTransient<MainPageViewModel>();

            builder.Services.AddTransientPopup<ImagePopup, ImageViewModel>();

            return builder.Build();
        }
    }
}
