using Android.App;
using Android.Content.PM;
using Android.OS;
using Com.Mapbox.Mapboxsdk;

namespace TFGVolandoVoy
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Inicializar Mapbox
            Mapbox.GetInstance(this, "pk.eyJ1IjoiZGF2aWRjbTI1NTk5IiwiYSI6ImNsdnAyMDNpcTAwNDAya3Ixb3l6YXZqZ3kifQ.jngETLaz7YtIwLOCE5kcDQ");

            // Otro código de inicialización de la actividad
        }
    }
}
