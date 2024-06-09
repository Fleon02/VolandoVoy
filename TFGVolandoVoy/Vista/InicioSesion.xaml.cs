using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using Microsoft.VisualBasic;
using System;
using System.IO;
using System.Net.Mail;
using System.Net;
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
            PasswordEntry.Text = "";
            userEntry.Text = "";
#if WINDOWS
        BtnLogin.HorizontalOptions = LayoutOptions.FillAndExpand;
#endif

#if ANDROID
        BtnLogin.WidthRequest = 200;
        BtnLogin.HeightRequest = 50;
#endif
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

        private async void IniciaSesion(object sender, EventArgs e)
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
                        AppShell.CurrentUser.Rol = usuarioExistente.Rol;
                        AppShell.CurrentUser.IdUsuario = usuarioExistente.IdUsuario;
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

        private void CambiarTapped(object sender, EventArgs e)
        {
            EnviarCorreoRecuPass();
        }

        public async void EnviarCorreoRecuPass()
        {
            //await Navigation.PushAsync(new CambiarPass("email"));

            string codigoRecibidoCorreo = "";
            // Obtener el nombre de usuario
            string email = await DisplayPromptAsync("Por favor, introduce tu email:", "Email");

            var usuariosConEmail = await _supabaseClient.From<Usuario>().Get();
            var usuarioExistente = usuariosConEmail.Models.FirstOrDefault(u => u.EmailUsuario == email);

            if (usuarioExistente == null)
            {
                await DisplayAlert("Error", "No existe un usuario con ese Correo Registrado", "OK");
                return;
            }

            // Crear una instancia de Random con una semilla basada en el tiempo actual
            Random random = new Random();

            // Definir los caracteres que pueden estar en el código
            const string caracteres = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

            // Crear una cadena para almacenar el código
            string codigo = "";

            // Generar un código aleatorio de 8 caracteres
            for (int i = 0; i < 8; i++)
            {
                // Obtener un índice aleatorio dentro del rango de caracteres
                int indice = random.Next(0, caracteres.Length);

                // Concatenar el carácter correspondiente al código
                codigo += caracteres[indice];
            }

            // Verificar si se proporcionó un nombre de usuario
            if (!string.IsNullOrEmpty(email))
            {
                string senderEmail = "tfgvolandovoy@gmail.com";
                string senderPassword = "efnq mfgn dego nrhg";

                string recipientEmail = email;
                string subject = "Cambio de Contraseña";
                string body = $"Has solicitado cambiar la contraseña. Utiliza el siguiente código en la app : " + codigo
                    + "\n Si no has solicitado nada, ignora este correo";

                // Configurar cliente SMTP de Gmail
                var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential(senderEmail, senderPassword),
                    EnableSsl = true,
                };

                // Crear mensaje de correo electrónico
                var message = new MailMessage(senderEmail, recipientEmail, subject, body);

                try
                {
                    // Enviar correo electrónico
                    smtpClient.Send(message);
                    await DisplayAlert("Éxito", "Correo electrónico enviado correctamente.", "OK");
                    do
                    {
                        

                        codigoRecibidoCorreo = await DisplayPromptAsync($"Por favor, introduce el código enviado a: {recipientEmail}", "Comprobación");

                        if (codigo.Equals(codigoRecibidoCorreo))
                        {
                            await DisplayAlert("Código Correcto", "Código Correcto", "OK");

                            await Navigation.PushAsync(new CambiarPass(email));

                            
                            break; // Salir del bucle si el código es correcto
                            
                        }
                        else
                        {
                            await DisplayAlert("Código Incorrecto", "Código Incorrecto", "OK");
                        }

                    } while (await DisplayAlert("Confirmación", "¿Quieres volver a intentarlo?", "Sí", "No"));

                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", "Error al enviar el correo electrónico: " + ex.Message, "OK");
                }
                finally
                {
                    // Liberar recursos
                    message.Dispose();
                    smtpClient.Dispose();
                }
            }
        
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            // Limpiar los campos de usuario y contraseña
            PasswordEntry.Text = "";
            userEntry.Text = "";
        }



    }
}
