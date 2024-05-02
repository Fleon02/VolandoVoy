using Microsoft.Maui.Controls;
using Microsoft.UI.Xaml;
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

             

            // Concatenar el mensaje de bienvenida con el nombre de usuario
            string welcomeMessage = "Bienvenid@ " + AppShell.CurrentUser.Username;
            Mensaje.Text = welcomeMessage;

            // Suscribir al evento PropertyChanged del modelo de usuario
            AppShell.CurrentUser.PropertyChanged += OnCurrentUserPropertyChanged;

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
    }
}
