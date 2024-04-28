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
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });


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
