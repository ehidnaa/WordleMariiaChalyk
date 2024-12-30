using Microsoft.Extensions.Logging;
using Wordle.Pages;

namespace Wordle
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp ()
        {
            var builder = MauiApp.CreateBuilder ();
            builder.UseMauiApp<App> ()
                .ConfigureFonts ( fonts =>
                {
                    fonts.AddFont ( "OpenSans-Regular.ttf", "OpenSansRegular" );
                    fonts.AddFont ( "OpenSans-Semibold.ttf", "OpenSansSemibold" );
                } );

            builder.Services.AddSingleton<Store> ();

#if DEBUG
            builder.Logging.AddDebug ();
#endif

            Routing.RegisterRoute ( "Menu", typeof ( MenuPage ) );
            Routing.RegisterRoute ( "Settings", typeof ( SettPage ) );
            Routing.RegisterRoute ( "Play", typeof ( PlayPage ) );
            Routing.RegisterRoute ( "GameSett", typeof (GameSettPage ) );

            return builder.Build ();
        }
    }
    //--------------------------------------------
    public static class Service
    {
        public static TService Get<TService> ()
            => IPlatformApplication.Current.Services.GetService<TService> ();
    }
}
