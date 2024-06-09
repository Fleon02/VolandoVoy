using Microsoft.Maui.Controls;
using Supabase.Interfaces;
using System.Net.Mail;
using System.Net;
using TFGVolandoVoy.Modelo;

namespace TFGVolandoVoy.Vista
{
    public partial class CambiarPass : ContentPage
    {
        private string emailUsuario;
        public CambiarPass(string email)
        {
            emailUsuario = email;
            InitializeComponent();
            lblTitle.Text = $"CAMBIANDO CONTRASEÑA DE {emailUsuario}";
            Shell.SetFlyoutBehavior(this, FlyoutBehavior.Disabled);
        }

        private async void BtnCambiarPass_Clicked(object sender, EventArgs e)
        {
            if (txtNewPassword.Text == txtRepeatPassword.Text)
            {
                Supabase.Client cliente = new Supabase.Client(ConexionSupabase.SUPABASE_URL, ConexionSupabase.SUPABASE_KEY);

                var usuariosConEmail = await cliente.From<Usuario>().Get();
                var usuarioExistente = usuariosConEmail.Models.FirstOrDefault(u => u.EmailUsuario == emailUsuario);

                var beepUsuario = await cliente.From<Beep>().Get();
                var beep = beepUsuario.Models.FirstOrDefault(u => u.IdUsuario == usuarioExistente.IdUsuario);

                string salt = PasswordEncoder.GenerateSalt();

                String nuevaContrasenaHasheada = PasswordEncoder.EncodePassword(txtNewPassword.Text, salt);

                await cliente.From<Beep>()
                    .Where(x => x.IdUsuario == usuarioExistente.IdUsuario)
                    .Set(x => x.Salt, salt)
                    .Set(x => x.HashContrasena, nuevaContrasenaHasheada)
                    .Update();

                await DisplayAlert("Éxito", "Contraseña Modificada", "Aceptar");
                EnviarCorreoConfimacion();
            }
            else
            {
                await DisplayAlert("Error", "Las Contraseñas no coinciden", "Aceptar");
                return;
            }


            
        }

        private async void EnviarCorreoConfimacion()
        {
            string senderEmail = "tfgvolandovoy@gmail.com";
            string senderPassword = "efnq mfgn dego nrhg";

            string recipientEmail = emailUsuario;
            string subject = "Cambio de Contraseña Confirmado VolandoVoy";
            string body = $"Se ha realizado el cambio de contraseña correctamente";

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
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Error al enviar el correo electrónico de Confirmacion: " + ex.Message, "OK");
            }
            finally
            {
                // Liberar recursos
                message.Dispose();
                smtpClient.Dispose();
            }
        }

        private void MostrarPassBoton(object sender, EventArgs e)
        {
            txtNewPassword.IsPassword = !txtNewPassword.IsPassword;
            MostrarPass();
            
        }

        private void MostrarPass()
        {
            var temaActual = App.Current.RequestedTheme;


            if (txtNewPassword.IsPassword == true)
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

        private void MostrarRepePassBoton(object sender, EventArgs e)
        {
            txtRepeatPassword.IsPassword = !txtRepeatPassword.IsPassword;
            MostrarRepePass();
        }

        private void MostrarRepePass()
        {
            var temaActual = App.Current.RequestedTheme;


            if (txtRepeatPassword.IsPassword == true)
            {
                // Establecer la imagen según el tema
                if (temaActual != AppTheme.Dark)
                {
                    imagenRepeBoton.Source = "visibledark.png";
                }
                else
                {
                    imagenRepeBoton.Source = "visible.png";
                }
            }
            else
            {
                // Establecer la imagen según el tema
                if (temaActual != AppTheme.Dark)
                {
                    imagenRepeBoton.Source = "invisibledark.png";
                }
                else
                {
                    imagenRepeBoton.Source = "invisible.png";
                }
            }
        }
    }
}
