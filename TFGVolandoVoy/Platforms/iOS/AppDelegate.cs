using Foundation;
using UIKit;

namespace TFGVolandoVoy
{
    [Register("AppDelegate")]
    public class AppDelegate : MauiUIApplicationDelegate
    {
        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            Mapbox.MGLAccountManager.AccessToken = "TU_TOKEN_DE_ACCESO_A_MAPBOX";

            return base.FinishedLaunching(app, options);
        }
    }
}
