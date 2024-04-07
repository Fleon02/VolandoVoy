using Microsoft.Maui.Controls;
using System;
using System.IO;
using System.Threading.Tasks;
using TFGVolandoVoy.Modelo;

namespace TFGVolandoVoy
{
    public partial class VntPrincipal : ContentPage
    {
        public VntPrincipal()
        {
            InitializeComponent();

            // Asignar la imagen y el texto directamente
            Imagen.Source = AppShell.CurrentUser.UserImage;
            Texto.Text = AppShell.CurrentUser.Username;

            // Concatenar el mensaje de bienvenida con el nombre de usuario
            string welcomeMessage = "Bienvenid@ a Volando Voy, " + AppShell.CurrentUser.Username;
            Mensaje.Text = welcomeMessage;

            // Suscribir al evento PropertyChanged del modelo de usuario
            AppShell.CurrentUser.PropertyChanged += OnCurrentUserPropertyChanged;
        }

        // Método que se ejecuta cuando cambia alguna propiedad del modelo de usuario
        private void OnCurrentUserPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            // Verificar qué propiedad cambió y actualizar la interfaz de usuario en consecuencia
            if (e.PropertyName == nameof(AppShell.CurrentUser.UserImage))
            {
                Imagen.Source = AppShell.CurrentUser.UserImage;
            }
            else if (e.PropertyName == nameof(AppShell.CurrentUser.Username))
            {
                Texto.Text = AppShell.CurrentUser.Username;
                // Concatenar el mensaje de bienvenida con el nombre de usuario
                string welcomeMessage = "Bienvenid@ a Volando Voy, " + AppShell.CurrentUser.Username;
                Mensaje.Text = welcomeMessage;
            }
        }
    }
}
