using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.Maps.MapControl.WPF.Overlays;
using System.Collections.ObjectModel;
using TFGVolandoVoy.Modelo;

namespace TFGVolandoVoy.Vista;

public partial class DetallesReto : ContentPage
{
   
    private readonly Supabase.Client _supabaseClient;
    private string nombre_imagen_preview="";
    private string nombre_imagen_completado="";
    private String? imagenElegida = null;
    private Reto? retoActual;


    public DetallesReto(Supabase.Client supabaseClient)

    {
        _supabaseClient = supabaseClient;       
        InitializeComponent();
        Shell.SetFlyoutBehavior(this, FlyoutBehavior.Disabled);
        if (Device.RuntimePlatform != Device.WinUI)
        {
            // Clear existing row and column definitions
            DetallesRetoGrid.RowDefinitions.Clear();
            DetallesRetoGrid.ColumnDefinitions.Clear();

            // Define row-based layout for Windows
            DetallesRetoGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            DetallesRetoGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            DetallesRetoGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            DetallesRetoGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            // Set controls to rows
            Grid.SetRow(l_antes, 0);
            Grid.SetRow(ImagenRetoPreviewDetalles, 1);
            Grid.SetRow(l_despues, 2);
            Grid.SetRow(ImagenCompletadoDetalles, 3);

            // Set controls to first column (default, single column layout)
            Grid.SetColumn(l_antes, 0);
            Grid.SetColumn(ImagenRetoPreviewDetalles, 0);
            Grid.SetColumn(l_despues, 0);
            Grid.SetColumn(ImagenCompletadoDetalles, 0);

            

        }

        
    }

    public DetallesReto(string resumenR) : this(new Supabase.Client(ConexionSupabase.SUPABASE_URL, ConexionSupabase.SUPABASE_KEY))
    {
        GetRetoByResumen(resumenR);
        labelResumen.Text = resumenR;

        
    }


    protected override void OnAppearing()
    {
        base.OnAppearing();


        
    }

    private async void GetRetoByResumen(string resumenR)
    {
        try
        {
            var retos = await _supabaseClient.From<Reto>().Get();
            var reto = retos.Models.FirstOrDefault(p => p.ResumenDeReto == resumenR);
            if (reto != null)
            {
                nombre_imagen_preview = reto.ImagenRetoPreview;
                nombre_imagen_completado = reto.ImagenCompletado;

                ImagenRetoPreviewDetalles.Source = nombre_imagen_preview;
                ImagenCompletadoDetalles.Source = nombre_imagen_completado;
                DescripcionRetoDetalles.Text = reto.DescripcionReto;
                if (!nombre_imagen_completado.Equals("https://clfynwobrskueprtvnmg.supabase.co/storage/v1/object/public/ImagenesRetoCompletado/reto_no_completado.png"))
                {
                    ActualizarReto.IsVisible = false;
                }
            }
            else
            {
                throw new Exception($"No se encontró ninguna imagen de {resumenR}");
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Error al obtener {resumenR}: {ex.Message}");
        }


    }

    private async void ActualizarRetoImgCompletado_Clicked(object sender, EventArgs e)
    {
        try
        {

            var retos = await _supabaseClient.From<Reto>().Get();
            var reto = retos.Models.FirstOrDefault(p => p.ResumenDeReto == labelResumen.Text);
            if (reto != null)
            {
                imagenElegida = await SeleccionarImagen();
                if (!string.IsNullOrEmpty(imagenElegida))
                {
                    Reto reto1 = reto;
                    reto1.Superado = true;
                    reto1.ImagenCompletado = imagenElegida;
                    ImagenCompletadoDetalles.Source = imagenElegida;
                    await _supabaseClient.From<Reto>().Update(reto1);
                    await DisplayAlert("Éxito", "Reto marcado como completado", "OK");

                }
            }
            
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"No se pudo actualizar el reto: {ex.Message}", "OK");
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


    private async Task<string> SeleccionarImagen()
    {
        var mediaFile = await MediaPicker.PickPhotoAsync();

        if (mediaFile != null)
        {
            byte[] fileBytes = await ReadFileAsBytes(mediaFile);

            if (fileBytes != null)
            {
                var fileName = $"imgRetoCompleto_{DateTime.Now.Ticks}.png";
                var response = await _supabaseClient.Storage.From("ImagenesRetoCompletado").Upload(fileBytes, fileName);

                if (response != null)
                {
                    string imageUrl = $"{ConexionSupabase.SUPABASE_URL}/storage/v1/object/public/ImagenesRetoCompletado/{fileName}";
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

    private async void ImagenRetoSuperado(object sender, EventArgs e)
    {
        imagenElegida = await SeleccionarImagen();
    }


}