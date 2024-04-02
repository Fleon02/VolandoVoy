using Microsoft.Maui.ApplicationModel;
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
            MostrarPass();
        }

        // Constructor sin parámetros
        public InicioSesion() : this(new Supabase.Client(ConexionSupabase.SUPABASE_URL, ConexionSupabase.SUPABASE_KEY))
        {
        }

        // Resto del código de la clase
        private int count = 0;


        private void MostrarPassBoton(object sender, EventArgs e)
        {
            PasswordEntry.IsPassword = !PasswordEntry.IsPassword;
            MostrarPass();
        }

        private void MostrarPass()
        {
            var temaActual = App.Current.RequestedTheme;

            
            if (PasswordEntry.IsPassword == true)
            {
                // Establecer la imagen según el tema
                if (temaActual != AppTheme.Dark)
                {
                    imagenBoton.Source = "visibledark.png";
                }
                else
                {
                    imagenBoton.Source = "visible.png";
                }
            }
            else
            {
                // Establecer la imagen según el tema
                if (temaActual != AppTheme.Dark)
                {
                    imagenBoton.Source = "invisibledark.png";
                }
                else
                {
                    imagenBoton.Source = "invisible.png";
                }
            }
        }

        private async void OnCounterClicked(object sender, EventArgs e)
        {
            // Define el correo electrónico y la contraseña para el registro
            string email = userEntry.Text;
            string password = PasswordEntry.Text;  

            try
            {
                if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password))
                {
                    var usuariosConEmail = await _supabaseClient.From<Usuario>().Get();
                    var usuarioExistente = usuariosConEmail.Models.FirstOrDefault(u => u.EmailUsuario == email);

                    if (usuarioExistente == null)
                    {
                        await DisplayAlert("Error", "El usuario no existe", "Aceptar");
                        return;
                    }

                    var beepUsuario = await _supabaseClient.From<Beep>().Get();
                    var beep = beepUsuario.Models.FirstOrDefault(u => u.IdUsuario == usuarioExistente.IdUsuario);

                    string salt = beep.Salt;

                    String contrasenaHasheada = PasswordEncoder.EncodePassword(password, salt);

                    if (beep.HashContrasena.Equals(contrasenaHasheada)){
                        await Shell.Current.GoToAsync("//VntPrincipal");
                        AppShell.CurrentUser.Username = usuarioExistente.NombreUsuario;
                        AppShell.CurrentUser.UserImage = usuarioExistente.ImagenUsuario;
                    }
                    else
                    {
                        await DisplayAlert("Error", "La contraseña es incorrecta", "Aceptar");
                    }
                }
                else
                {
                    await DisplayAlert("Error", "Por favor complete todos los campos obligatorios.", "Aceptar");
                }


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
