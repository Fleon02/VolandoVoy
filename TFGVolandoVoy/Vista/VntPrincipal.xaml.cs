using Microsoft.Maui.Controls;
using System;
using System.IO;
using System.Threading.Tasks;
using TFGVolandoVoy.Modelo;
using Microsoft.Maui.Controls.Maps;
using Map = Microsoft.Maui.Controls.Maps.Map;
using Microsoft.Maui.Maps; // Asegúrate de incluir esta línea

namespace TFGVolandoVoy
{
    public partial class VntPrincipal : ContentPage
    {
        public VntPrincipal()
        {
            InitializeComponent();

             

            // Concatenar el mensaje de bienvenida con el nombre de usuario
            string welcomeMessage = "Bienvenid@ " + AppShell.CurrentUser.Username;
            Mensaje.Text = welcomeMessage;

            // Suscribir al evento PropertyChanged del modelo de usuario
            AppShell.CurrentUser.PropertyChanged += OnCurrentUserPropertyChanged;

#if __ANDROID__
            AddMap();
#endif

#if WINDOWS
            AddMapBing();
#endif

        }

        // Método que se ejecuta cuando cambia alguna propiedad del modelo de usuario
        private void OnCurrentUserPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(AppShell.CurrentUser.Username))
            {
               
                // Concatenar el mensaje de bienvenida con el nombre de usuario
                string welcomeMessage = "Bienvenid@ " + AppShell.CurrentUser.Username;
                Mensaje.Text = welcomeMessage;
            }
        }

        private void AddMap()
        {
            var mapView = new Map
            {
                HeightRequest = 300,
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                IsShowingUser = true,
                IsScrollEnabled = true,
                MapType = MapType.Street
            };

            stacklayout.Children.Add(mapView);
        }

private void AddMapBing()
{
    var mapView = new Microsoft.Maui.Controls.Maps.Map
    {
        HeightRequest = 300,
        VerticalOptions = LayoutOptions.FillAndExpand,
        HorizontalOptions = LayoutOptions.FillAndExpand,
    };

            // Configura la ubicación y el nivel de zoom inicial
            mapView.MoveToRegion(MapSpan.FromCenterAndRadius(new Location(40.4168, -3.7038), Distance.FromKilometers(1)));

    stacklayout.Children.Add(mapView);
}


    }
}
