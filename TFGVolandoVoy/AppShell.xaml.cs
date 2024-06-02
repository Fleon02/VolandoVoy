namespace TFGVolandoVoy
{
    public partial class AppShell : Shell
    {
        public static UsuarioModel CurrentUser { get; set; } = new UsuarioModel();
        public static AppShell Current;

        public AppShell()
        {

            InitializeComponent();

            Current = this;

            Routing.RegisterRoute("InicioSesion", typeof(InicioSesion));
            Routing.RegisterRoute("LocalidadVnt", typeof(LocalidadVnt));
            Routing.RegisterRoute("CerrarSesion", typeof(InicioSesion));
            Routing.RegisterRoute("RetosVnt", typeof(RetosVnt));
            BindingContext = CurrentUser;


            GoToAsync("InicioSesion");
            SetAppIcon();
            Application.Current.RequestedThemeChanged += OnRequestedThemeChanged;

        }


        private void OnRequestedThemeChanged(object sender, AppThemeChangedEventArgs e)
        {
            SetAppIcon();
        }

        private void SetAppIcon()
        {
            if (Application.Current.RequestedTheme == AppTheme.Dark)
            {
                this.Resources["IconoInicio"] = "inicio.png";
                this.Resources["IconoRetos"] = "reto.png";
                this.Resources["IconoLocalidad"] = "map.png";
                this.Resources["IconoLogout"] = "logout.png";
            }
            else
            {
                this.Resources["IconoInicio"] = "iniciodark.png";
                this.Resources["IconoRetos"] = "retodark.png";
                this.Resources["IconoLocalidad"] = "mapdark.png";
                this.Resources["IconoLogout"] = "logoutdark.png";
            }
        }

        public async Task NavigateToPage(string route)
        {

            Page page = null;

            switch (route)
            {
                case "InicioSesion":
                    page = new InicioSesion();
                    break;
                case "LocalidadVnt":
                    page = new LocalidadVnt();
                    break;
                case "RetosVnt":
                    page = new RetosVnt();
                    break;
            }

            if (page != null)
            {
                await Navigation.PushAsync(page);
            }
        }
    }
}
