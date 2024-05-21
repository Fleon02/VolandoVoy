using Microsoft.Maps.MapControl.WPF.Overlays;
using Supabase;
using Supabase.Interfaces;
using System.Collections.ObjectModel;
using TFGVolandoVoy.Modelo;

namespace TFGVolandoVoy.Vista;

public partial class VntRetos : ContentPage
{
    private readonly Supabase.Client _supabaseClient;
    public VntRetos(Supabase.Client supabaseClient)
	{
        _supabaseClient = supabaseClient;
        InitializeComponent();
        Shell.SetFlyoutBehavior(this, FlyoutBehavior.Disabled);
    }

    public VntRetos(string nombrelocalidad) : this(new Supabase.Client(ConexionSupabase.SUPABASE_URL, ConexionSupabase.SUPABASE_KEY))
    {

        //GetLocalidadById(idLocalidad);
        CargarRetos(nombrelocalidad);
    }

    private async void CargarRetos(string nombrelocalidad)
    {
        try
        {
            var localidades = await _supabaseClient.From<Localidad>().Get();
            var localidad = localidades.Models.FirstOrDefault(p => p.NombreLocalidad == nombrelocalidad);
            var retos = await _supabaseClient.From<Reto>().Get();
            var reto = retos.Models.Where(p => p.IdLocalidad == localidad.IdLocalidad).ToList();
            if (reto != null)
            {
                //ListaRetos.ItemsSource = new ObservableCollection<Reto>(reto);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error al cargar los comentarios: {ex.Message}", "Aceptar");
        }
    }
}