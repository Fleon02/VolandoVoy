using Microsoft.Maps.MapControl.WPF.Overlays;
using Supabase;
using Supabase.Interfaces;
using System.Collections.ObjectModel;
using TFGVolandoVoy.Modelo;

namespace TFGVolandoVoy.Vista;

public partial class Retos : ContentPage
{
    private readonly Supabase.Client _supabaseClient;
    private long idLocalidad;
    

    public string TextoReto { get; set; }

    public Retos(Supabase.Client supabaseClient)
    {
        _supabaseClient = supabaseClient;
        // Configuración adicional si es necesaria antes de inicializar los componentes
        InitializeComponent();
    }


    public Retos() : this(new Supabase.Client(ConexionSupabase.SUPABASE_URL, ConexionSupabase.SUPABASE_KEY))
    {
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();        

        //NombreLocalidad.Text = "Localidad : ";
        //NombreProvincia.Text = "Provincia : ";
        CargarRetos();
    }

    private async void CargarRetos()
    {
        try
        {
            // Obtener la lista de retos desde la base de datos
            var retos = await _supabaseClient.From<Retoss>().Get();
            //LocalidadesListView.ItemsSource = new ObservableCollection<Localidad>(localidades.Models);
            ListaRetos.ItemsSource = new ObservableCollection<Retoss>(retos.Models);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error al cargar los retos: {ex.Message}", "Aceptar");
        }
    }

    private void DetallesJ(object sender, TappedEventArgs e)
    {

    }

    private void Completar_Clicked(object sender, EventArgs e)
    {

    }
}