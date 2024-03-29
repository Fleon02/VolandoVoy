using Microsoft.Maui.Controls;
using System;
using System.IO;
using System.Threading.Tasks;
using TFGVolandoVoy.Modelo;
using TFGVolandoVoy.Vista;

namespace TFGVolandoVoy
{
    public partial class InicioSesion : ContentPage
    {
        private readonly Supabase.Client _supabaseClient;

        // Constructor con parámetro
        public InicioSesion(Supabase.Client supabaseClient)
        {
            _supabaseClient = supabaseClient;
            InitializeComponent();
        }

        // Constructor sin parámetros
        public InicioSesion() : this(new Supabase.Client(ConexionSupabase.SUPABASE_URL, ConexionSupabase.SUPABASE_KEY))
        {
        }

        // Resto del código de la clase
        private int count = 0;


        private void MostrarPass(object sender, EventArgs e)
        {
            PasswordEntry.IsPassword = !PasswordEntry.IsPassword;
            if (PasswordEntry.IsPassword == true)
            {
                imagenBoton.Source = "visible.png";
            }else
            {
                imagenBoton.Source = "invisible.png";
            }
        }

        private async void OnCounterClicked(object sender, EventArgs e)
        {
            // Define el correo electrónico y la contraseña para el registro
            string email = "zxxwaspxxz@gmail.com";
            string password = "contraseña123";

            try
            {
                // Registra al usuario utilizando el correo electrónico y la contraseña predefinidos
                //var response = await _supabaseClient.Auth.SignUp(email, password);

                await Shell.Current.GoToAsync("//ProvinciaVnt");
            }
            catch (Exception ex)
            {
                // Si ocurre alguna excepción durante el registro, muestra un mensaje de error
                await DisplayAlert("Error", $"Error al registrar al usuario: {ex.Message}", "Aceptar");
            }
        }

        private void OnRegistroTapped(object sender, EventArgs e)
        {
            // Aquí navegas a la ventana de registro
            Navigation.PushAsync(new Registro());
        }


    }
}
