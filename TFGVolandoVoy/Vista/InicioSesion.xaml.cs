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

        
        public InicioSesion() : this(new Supabase.Client(ConexionSupabase.SUPABASE_URL, ConexionSupabase.SUPABASE_KEY))
        {
        }

        
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
                
                await DisplayAlert("Error", $"Error al registrar al usuario: {ex.Message}", "Aceptar");
            }
        }

        private void OnRegistroTapped(object sender, EventArgs e)
        {
            
            Navigation.PushAsync(new Registro());
        }

        private void CambiarTapped(object sender, EventArgs e)
        {
            EnviarCorreoRecuPass();
        }

        public async void EnviarCorreoRecuPass()
        {
            

            string codigoRecibidoCorreo = "";
           
            string email = await DisplayPromptAsync("Por favor, introduce tu email:", "Email");

            var usuariosConEmail = await _supabaseClient.From<Usuario>().Get();
            var usuarioExistente = usuariosConEmail.Models.FirstOrDefault(u => u.EmailUsuario == email);

            if (usuarioExistente == null)
            {
                await DisplayAlert("Error", "No existe un usuario con ese Correo Registrado", "OK");
                return;
            }

            
            Random random = new Random();

            
            const string caracteres = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

            
            string codigo = "";

            
            for (int i = 0; i < 8; i++)
            {
                
                int indice = random.Next(0, caracteres.Length);

                
                codigo += caracteres[indice];
            }

            
            if (!string.IsNullOrEmpty(email))
            {
                string senderEmail = "tfgvolandovoy@gmail.com";
                string senderPassword = "efnq mfgn dego nrhg";

                string recipientEmail = email;
                string subject = "Cambio de Contraseña";
                string body = $"Has solicitado cambiar la contraseña. Utiliza el siguiente código en la app : " + codigo
                    + "\n Si no has solicitado nada, ignora este correo";

               
                var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential(senderEmail, senderPassword),
                    EnableSsl = true,
                };

                
                var message = new MailMessage(senderEmail, recipientEmail, subject, body);

                try
                {
                    
                    smtpClient.Send(message);
                    await DisplayAlert("Éxito", "Correo electrónico enviado correctamente.", "OK");
                    do
                    {
                        

                        codigoRecibidoCorreo = await DisplayPromptAsync($"Por favor, introduce el código enviado a: {recipientEmail}", "Comprobación");

                        if (codigo.Equals(codigoRecibidoCorreo))
                        {
                            await DisplayAlert("Código Correcto", "Código Correcto", "OK");

                            await Navigation.PushAsync(new CambiarPass(email));

                            
                            break;
                            
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
                    
                    message.Dispose();
                    smtpClient.Dispose();
                }
            }
        
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            
            PasswordEntry.Text = "";
            userEntry.Text = "";
        }



    }
}
