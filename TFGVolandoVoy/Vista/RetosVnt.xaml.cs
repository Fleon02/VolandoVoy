using Supabase.Interfaces;
using TFGVolandoVoy.Modelo;

namespace TFGVolandoVoy;

public partial class RetosVnt : ContentPage
{
    private readonly Supabase.Client _supabaseClient;
    public RetosVnt(Supabase.Client supabaseClient)
	{
        _supabaseClient = supabaseClient;
        InitializeComponent();
        _ = CargarLocalidades();
    }

    public RetosVnt() : this(new Supabase.Client(ConexionSupabase.SUPABASE_URL, ConexionSupabase.SUPABASE_KEY)) 
    {
    }

    private async void selector_ciudades_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (selector_ciudades.SelectedItem is Localidad selectedLocalidad)
        {
            await ActualizarRetos(selectedLocalidad.IdLocalidad);
        }
    }

    private async Task ActualizarRetos(long idLocalidad)
    {
        var response = await _supabaseClient.From<Reto>().Where(r => r.IdLocalidad == idLocalidad).Get();
        var retos = response.Models;

        lista_de_retos.ItemsSource = retos;
    }

    private async Task CargarLocalidades()
    {
        var response = await _supabaseClient.From<Localidad>().Get();
        var localidades = response.Models;

        if (localidades != null && localidades.Count > 0)
        {
            foreach (var localidad in localidades)
            {
                Console.WriteLine($"Localidad: {localidad.NombreLocalidad}");
            }
        }
        else
        {
            Console.WriteLine("No se encontraron localidades.");
        }

        selector_ciudades.ItemsSource = localidades;
    }

}