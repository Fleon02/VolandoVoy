using Supabase.Interfaces;
using TFGVolandoVoy.Modelo;
using System.Text.RegularExpressions;
using System.Net.Mail;
using System.Net;

namespace TFGVolandoVoy.Vista;

public partial class Registro : ContentPage
{
    private readonly Supabase.Client _supabaseClient;
    String? imagenElegida = null;
    public Registro(Supabase.Client supabaseClient)
    {
        _supabaseClient = supabaseClient;
        InitializeComponent();
        Shell.SetFlyoutBehavior(this, FlyoutBehavior.Disabled);
    }

    // Constructor sin parámetros
    public Registro() : this(new Supabase.Client(ConexionSupabase.SUPABASE_URL, ConexionSupabase.SUPABASE_KEY))
    {
    }

    private async void OnRegistrarClicked(object sender, EventArgs e)
    {
        var nombre = CampoNombre.Text;
        var apellidos = CampoApellidos.Text;
        var email = CampoEmail.Text;
        var contrasena = PasswordEntry.Text;
        var imagen = imagenElegida;

        if (imagen == null)
        {
            imagen = "https://clfynwobrskueprtvnmg.supabase.co/storage/v1/object/public/perfilesIMG/user-48.png";
        }

        if (string.IsNullOrEmpty(nombre) || string.IsNullOrEmpty(apellidos) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(contrasena))
        {
            await DisplayAlert("Error", "Por favor complete todos los campos obligatorios.", "Aceptar");
            return;
        }

        if (!IsValidEmail(email))
        {
            await DisplayAlert("Error", "El correo electrónico no es válido. Por favor, utilice otro.", "Aceptar");
            return;
        }

        var usuariosConEmail = await _supabaseClient.From<Usuario>().Get();
        var usuarioExistente = usuariosConEmail.Models.FirstOrDefault(u => u.EmailUsuario == email);

        if (usuarioExistente != null)
        {
            await DisplayAlert("Error", "El correo electrónico ya está registrado. Por favor, utilice otro.", "Aceptar");
            return;
        }

        if (!PasswordEntry.Text.Equals(RepetirPasswordEntry.Text))
        {
            await DisplayAlert("Error", "Las Contraseñas no coinciden", "Aceptar");
            return;
        }


        // Generate confirmation code
        string confirmationCode = GenerateConfirmationCode();

        // Send confirmation email
        await SendConfirmationEmail(email, confirmationCode);

        bool isCodeValid = false;

        while (!isCodeValid)
        {
            // Prompt user to enter the confirmation code
            string enteredCode = await DisplayPromptAsync("Código de Confirmación", "Ingrese el código de confirmación enviado a su correo electrónico:", "Confirmar", "Cancelar", "Código de confirmación", maxLength: 6, keyboard: Keyboard.Numeric);

            if (enteredCode == null)
            {
                await DisplayAlert("Cancelado", "El registro ha sido cancelado.", "Aceptar");
                return;
            }

            if (enteredCode == confirmationCode)
            {
                Usuario u = new Usuario
                {
                    NombreUsuario = nombre,
                    ApellidosUsuario = apellidos,
                    EmailUsuario = email,
                    ImagenUsuario = imagen,
                    FechaAlta = DateTime.Now
                };

                await InsertUsuario(u, contrasena);
                isCodeValid = true;
            }
            else
            {
                await DisplayAlert("Error", "El código de confirmación no es válido. Por favor, inténtelo de nuevo.", "Aceptar");
            }
        }
    }



    private void MostrarPass(object sender, EventArgs e)
    {
        PasswordEntry.IsPassword = !PasswordEntry.IsPassword;
        imagenBoton.Source = PasswordEntry.IsPassword ? "visible.png" : "invisible.png";
    }

    private void MostrarRepePass(object sender, EventArgs e)
    {
        RepetirPasswordEntry.IsPassword = !RepetirPasswordEntry.IsPassword;
        imagenRepeBoton.Source = RepetirPasswordEntry.IsPassword ? "visible.png" : "invisible.png";
    }

    private async void Imagen(object sender, EventArgs e)
    {
        imagenElegida = await SeleccionarImagen();
    }

    private async Task<string> SeleccionarImagen()
    {
        var mediaFile = await MediaPicker.PickPhotoAsync();

        if (mediaFile != null)
        {
            byte[] fileBytes = await ReadFileAsBytes(mediaFile);

            if (fileBytes != null)
            {
                var fileName = $"imgUsuario_{DateTime.Now.Ticks}.png";
                var response = await _supabaseClient.Storage.From("perfilesIMG").Upload(fileBytes, fileName);

                if (response != null)
                {
                    string imageUrl = $"{ConexionSupabase.SUPABASE_URL}/storage/v1/object/public/perfilesIMG/{fileName}";
                    return imageUrl;
                }
                else
                {
                    await DisplayAlert("Error", "No se pudo cargar la imagen.", "Aceptar");
                }
            }
        }

        return "null";
    }

    private async Task<byte[]> ReadFileAsBytes(FileResult file)
    {
        using (var stream = await file.OpenReadAsync())
        {
            using (var memoryStream = new MemoryStream())
            {
                await stream.CopyToAsync(memoryStream);
                return memoryStream.ToArray();
            }
        }
    }

    private async Task InsertUsuario(Usuario usuario, string contrasena)
    {
        try
        {

            var count = await _supabaseClient.From<Usuario>().Select(x => new object[] { x.IdUsuario }).Count(Postgrest.Constants.CountType.Exact);

            usuario.Rol = count == 0 ? "admin" : "usuario";

            string salt = PasswordEncoder.GenerateSalt();
            string contrasenaHasheada = PasswordEncoder.EncodePassword(contrasena, salt);

            Beep b = new Beep
            {
                IdUsuario = usuario.IdUsuario,
                HashContrasena = contrasenaHasheada,
                Salt = salt
            };

            await _supabaseClient.From<Usuario>().Insert(usuario);
            await _supabaseClient.From<Beep>().Insert(b);
            await DisplayAlert("Éxito", "Usuario insertado correctamente.", "Aceptar");
            ResetFields();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"No se pudo insertar el usuario: {ex.Message}", "Aceptar");
        }
    }

    private void ResetFields()
    {
        CampoNombre.Text = "";
        CampoApellidos.Text = "";
        CampoEmail.Text = "";
        PasswordEntry.Text = "";
        RepetirPasswordEntry.Text = "";
    }

    private bool IsValidEmail(string email)
    {
        const string emailRegex = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        return Regex.IsMatch(email, emailRegex);
    }

    private string GenerateConfirmationCode()
    {
        Random random = new Random();
        return random.Next(100000, 999999).ToString();
    }

    private async Task SendConfirmationEmail(string email, string confirmationCode)
    {
        string senderEmail = "tfgvolandovoy@gmail.com";
        string senderPassword = "efnq mfgn dego nrhg";

        string recipientEmail = email;
        string subject = "Código de Confirmación VolandoVoy";
        string body = $"Su código de confirmación es: {confirmationCode}";

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
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", "Error al enviar el correo electrónico de confirmación: " + ex.Message, "OK");
        }
        finally
        {
            message.Dispose();
            smtpClient.Dispose();
        }
    }
}

