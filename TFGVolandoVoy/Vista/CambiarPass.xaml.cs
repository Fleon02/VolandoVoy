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
            lblTitle.Text = $"CAMBIANDO CONTRASE�A DE {emailUsuario}";
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

                await DisplayAlert("�xito", "Contrase�a Modificada", "Aceptar");
                EnviarCorreoConfimacion();
            }
            else
            {
                await DisplayAlert("Error", "Las Contrase�as no coinciden", "Aceptar");
                return;
            }


            
        }

        private async void EnviarCorreoConfimacion()
        {
            string senderEmail = "tfgvolandovoy@gmail.com";
            string senderPassword = "efnq mfgn dego nrhg";

            string recipientEmail = emailUsuario;
            string subject = "Cambio de Contrase�a Confirmado VolandoVoy";
            string body = $"Se ha realizado el cambio de contrase�a correctamente";

            // Configurar cliente SMTP de Gmail
            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(senderEmail, senderPassword),
                EnableSsl = true,
            };

            // Crear mensaje de correo electr�nico
            var message = new MailMessage(senderEmail, recipientEmail, subject, body);

            try
            {
                // Enviar correo electr�nico
                smtpClient.Send(message);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Error al enviar el correo electr�nico de Confirmacion: " + ex.Message, "OK");
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
                // Establecer la imagen seg�n el tema
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
                // Establecer la imagen seg�n el tema
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
                // Establecer la imagen seg�n el tema
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
                // Establecer la imagen seg�n el tema
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
