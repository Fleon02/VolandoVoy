using Supabase.Interfaces;
using TFGVolandoVoy.Modelo;

namespace TFGVolandoVoy.Vista;

public partial class Registro : ContentPage
{
    private readonly Supabase.Client _supabaseClient;
    String? imagenElegida = null;
    public Registro(Supabase.Client supabaseClient)
    {
        _supabaseClient = supabaseClient;
        InitializeComponent();
    }

    // Constructor sin par�metros
    public Registro() : this(new Supabase.Client(ConexionSupabase.SUPABASE_URL, ConexionSupabase.SUPABASE_KEY))
    {
    }

    private async void OnRegistrarClicked(object sender, EventArgs e)
    {
        if (PasswordEntry.Text.Equals(RepetirPasswordEntry.Text))
        {
            var nombre = CampoNombre.Text;
            var apellidos = CampoApellidos.Text;
            var email = CampoEmail.Text;
            var contrasena = PasswordEntry.Text;
            var imagen = imagenElegida;

            if (!string.IsNullOrEmpty(nombre) && !string.IsNullOrEmpty(apellidos) && !string.IsNullOrEmpty(imagen) && !string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(contrasena))
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
            }
            else
            {
                await DisplayAlert("Error", "Por favor complete todos los campos obligatorios.", "Aceptar");
            }
        }
        else
        {
            await DisplayAlert("Error", "Las contrase�as no coinciden.", "Aceptar");
        }
        
    }

    private void MostrarPass(object sender, EventArgs e)
    {
        PasswordEntry.IsPassword = !PasswordEntry.IsPassword;
        if (PasswordEntry.IsPassword == true)
        {
            imagenBoton.Source = "visible.png";
        }
        else
        {
            imagenBoton.Source = "invisible.png";
        }
    }
    private void MostrarRepePass(object sender, EventArgs e)
    {
        RepetirPasswordEntry.IsPassword = !RepetirPasswordEntry.IsPassword;
        if (RepetirPasswordEntry.IsPassword == true)
        {
            imagenRepeBoton.Source = "visible.png";
        }
        else
        {
            imagenRepeBoton.Source = "invisible.png";
        }
    }

    private async void Imagen(object sender, EventArgs e)
    {
        imagenElegida = await SeleccionarImagen();
    }


    private async Task<string> SeleccionarImagen()
    {
        // Subir imagen desde el dispositivo del usuario
        var mediaFile = await MediaPicker.PickPhotoAsync();

        if (mediaFile != null)
        {
            // Leer el contenido del archivo como un arreglo de bytes
            byte[] fileBytes = await ReadFileAsBytes(mediaFile);

            if (fileBytes != null)
            {
                // Subir el archivo al bucket en Supabase
                var fileName = $"imgUsuario_{DateTime.Now.Ticks}.png"; // Nombre �nico para el archivo
                var response = await _supabaseClient.Storage
                    .From("perfilesIMG")
                    .Upload(fileBytes, fileName);

                if (response != null)
                {
                    // Construir manualmente la URL de la imagen cargada
                    string imageUrl = $"{ConexionSupabase.SUPABASE_URL}/storage/v1/object/public/perfilesIMG/{fileName}";
                    return imageUrl;
                }
                else
                {
                    // Mostrar un mensaje de error si la carga de la imagen falla
                    await DisplayAlert("Error", "No se pudo cargar la imagen.", "Aceptar");
                }
            }
        }

        return "null";
    }

    // M�todo para leer el contenido del archivo como un arreglo de bytes
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

            var count = await _supabaseClient
              .From<Usuario>()
              .Select(x => new object[] { x.IdUsuario })
              .Count(Postgrest.Constants.CountType.Exact);

            if (count == 0)
            {
                usuario.Rol = "admin";
            } else
            {
                usuario.Rol = "usuario";
            }

            Beep b = new Beep
            {
                IdUsuario = usuario.IdUsuario,
                HashContrasena = contrasena,
                Salt = "1234"
            };

            await _supabaseClient.From<Usuario>().Insert(usuario);
            await _supabaseClient.From<Beep>().Insert(b);
            await DisplayAlert("�xito", "Usuario insertado correctamente.", "Aceptar");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"No se pudo insertar el usuario: {ex.Message}", "Aceptar");
        }
    }
}