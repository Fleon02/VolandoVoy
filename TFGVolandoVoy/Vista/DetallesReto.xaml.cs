using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Collections.ObjectModel;
using TFGVolandoVoy.Modelo;

namespace TFGVolandoVoy.Vista;

public partial class DetallesReto : ContentPage
{
   
    private readonly Supabase.Client _supabaseClient;
    private string nombre_imagen_preview="";
    private string nombre_imagen_completado="";
    
    
    public DetallesReto(Supabase.Client supabaseClient)

    {
        _supabaseClient = supabaseClient;       
        InitializeComponent();
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

    private void OnRetoSuperado(object sender, EventArgs e)
    {
        DisplayAlert("Botón Presionado", "El botón ha sido presionado", "OK");
    }

    

    public DetallesReto(string resumenR) : this(new Supabase.Client(ConexionSupabase.SUPABASE_URL, ConexionSupabase.SUPABASE_KEY))
    {
        GetRetoByResumen(resumenR);

        if(nombre_imagen_completado.Equals("https://clfynwobrskueprtvnmg.supabase.co/storage/v1/object/public/ImagenesRetoCompletado/reto_no_completado.png"))
        {
            Button boton = new Button
            {
                Text = "Reto completado",
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center
            };

            boton.Clicked += OnRetoSuperado;

            // Agregar el botón al layout
            DetallesRetoGrid.Children.Add(boton);
        }
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
}