using Microsoft.Maui.ApplicationModel.Communication;
using TFGVolandoVoy.Modelo;

namespace TFGVolandoVoy;

public partial class VentanaPerfil : ContentPage
{
    private readonly Supabase.Client _supabaseClient;
    private String imagenElegida;
    private byte[] imagenElegidaBytes;
    private bool imagenCambiada = false;

    private Usuario usuarioExistente;

    
    public VentanaPerfil(Supabase.Client supabaseClient)
    {
        InitializeComponent();
        _supabaseClient = supabaseClient;

        cargarDatosUsuario(AppShell.CurrentUser.IdUsuario);
    }

    public VentanaPerfil() : this(new Supabase.Client(ConexionSupabase.SUPABASE_URL, ConexionSupabase.SUPABASE_KEY))
    {
    }


    private async void cargarDatosUsuario(long idUsuario)
    {

        var usuario = await _supabaseClient.From<Usuario>().Get();
        usuarioExistente = usuario.Models.FirstOrDefault(u => u.IdUsuario == idUsuario);

        if (usuarioExistente != null)
        {
            NombreUsuarioEntry.Text = usuarioExistente.NombreUsuario;
            ApellidoEntry.Text = usuarioExistente.ApellidosUsuario;
            ImagenUsuario.Source = usuarioExistente.ImagenUsuario;
            imagenElegida = usuarioExistente.ImagenUsuario;
        }
        
    }

    private async void CambiarImagen(object sender, EventArgs e)
    {
        await SeleccionarImagen();


        ImagenUsuario.Source = ImageSource.FromStream(() => new MemoryStream(imagenElegidaBytes));

    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        imagenElegida = null;
        imagenElegidaBytes = null;
        imagenCambiada = false;

        cargarDatosUsuario(AppShell.CurrentUser.IdUsuario);
    }

    private async Task SeleccionarImagen()
    {
        var mediaFile = await MediaPicker.PickPhotoAsync();

        if (mediaFile != null)
        {
            imagenElegidaBytes = await ReadFileAsBytes(mediaFile);
            imagenCambiada = true;

        }
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

    private async void confirmarCambios_Clicked(object sender, EventArgs e)
    {
        await confirmarCambiosBBDD();

    }

    private async Task confirmarCambiosBBDD()
    {

        if (imagenCambiada == false)
        {
            await _supabaseClient.From<Usuario>()
            .Where(x => x.IdUsuario == usuarioExistente.IdUsuario)
            .Set(x => x.NombreUsuario, NombreUsuarioEntry.Text)
            .Set(x => x.ApellidosUsuario, ApellidoEntry.Text)
            .Update();

            await DisplayAlert("Éxito", "Cambios Realizados", "Aceptar");

        }else {

            if (imagenElegidaBytes != null)
            {
                var fileName = $"imgUsuario_{DateTime.Now.Ticks}.png";
                var response = await _supabaseClient.Storage.From("perfilesIMG").Upload(imagenElegidaBytes, fileName);

                if (response != null)
                {
                    string imageUrl = $"{ConexionSupabase.SUPABASE_URL}/storage/v1/object/public/perfilesIMG/{fileName}";

                    imagenElegida = imageUrl;


                    await _supabaseClient.From<Usuario>()
                    .Where(x => x.IdUsuario == usuarioExistente.IdUsuario)
                    .Set(x => x.NombreUsuario, NombreUsuarioEntry.Text)
                    .Set(x => x.ApellidosUsuario, ApellidoEntry.Text)
                    .Set(x => x.ImagenUsuario, imageUrl)
                    .Update();

                    

                    await DisplayAlert("Éxito", "Cambios Realizados, se reflejaran cuando inicies una nueva sesión", "Aceptar");
                }
                else
                {
                    await DisplayAlert("Error", "No se pudo cargar la imagen.", "Aceptar");
                    
                }
            }
        }

    }
}
