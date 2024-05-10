using CommunityToolkit.Maui.Maps;
using Microsoft.Extensions.Logging;
using Supabase;
using TFGVolandoVoy.Modelo;

namespace TFGVolandoVoy
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                //.UseMauiMaps() //quitar temporalmente si quieres ejecutar en windows
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("Oswald-Regular.ttf", "OswaldRegular");
                });

            // Agrega UseMauiMaps() solo si se está ejecutando en Android

#if __ANDROID__
            builder.UseMauiMaps();
#endif

#if WINDOWS
            builder.UseMauiCommunityToolkitMaps("iPtbFsMmeY5CX7BMJoqY~9oP6MVArNQXNgfOIBDO9yQ~AhC2-ljhL6etjcuGwGwMfcu41erkPiNDxwBIBz-Mg3C8puZ5rM7yjQWE-6_eyPSN");
#endif


            var url = ConexionSupabase.SUPABASE_URL;
            var key = ConexionSupabase.SUPABASE_KEY;

            var options = new SupabaseOptions
            {
                AutoRefreshToken = true,
                AutoConnectRealtime = true,
                //SessionHandler = new SupabaseSessionHandler()
            };
            builder.Services.AddSingleton(provider => new Supabase.Client(url, key, options));

#if DEBUG
            builder.Logging.AddDebug();

#endif

            return builder.Build();
        }
    }
}
