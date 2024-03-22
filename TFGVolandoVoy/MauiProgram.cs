using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Supabase;

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

            // Configurar Supabase
            // Configurar Supabase
            var url = "https://clfynwobrskueprtvnmg.supabase.co";
            var key = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6ImNsZnlud29icnNrdWVwcnR2bm1nIiwicm9sZSI6ImFub24iLCJpYXQiOjE3MTExMjI4OTYsImV4cCI6MjAyNjY5ODg5Nn0.yil-jPNh7m6uk9veYRnnAB2Cjt51lTyCbu18oiluk98";

            var options = new SupabaseOptions
            {
                AutoRefreshToken = true,
                AutoConnectRealtime = true,
                //SessionHandler = new SupabaseSessionHandler()
            };
            var supabase = new Supabase.Client(url, key, options);
            builder.Services.AddSingleton(supabase);

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
