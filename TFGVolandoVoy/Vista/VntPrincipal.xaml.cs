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
                IsShowingUser = false,
                IsScrollEnabled = false,
                IsZoomEnabled = false,
                
                MapType = MapType.Street
            };

            Pin mediasetPin = new Pin
            {
                Label = "Volando Voy - Mediaset", // Nombre del marcador               
                Location = new Location(40.5124429, -3.6810871)
            };
            mapView.Pins.Add(mediasetPin);

            mapView.MoveToRegion(MapSpan.FromCenterAndRadius(new Location(40.5124429, -3.6810871), Distance.FromKilometers(1)));
            stacklayout.Children.Add(mapView);

        }

        private void AddMapBing()
        {
            var mapView = new Microsoft.Maui.Controls.Maps.Map
            {
                HeightRequest = 300,
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                IsShowingUser = false,
                IsScrollEnabled = false,
                IsZoomEnabled = false,
            };
            Pin mediasetPin = new Pin
            {
                Label = "Volando Voy - Mediaset", // Nombre del marcador               
                Location = new Location(40.5124429, -3.6810871)
            };
            mapView.Pins.Add(mediasetPin);

            // Configura la ubicación y el nivel de zoom inicial
            mapView.MoveToRegion(MapSpan.FromCenterAndRadius(new Location(40.5124429, -3.6810871), Distance.FromKilometers(1)));

            stacklayout.Children.Add(mapView);

        }


    }
}
