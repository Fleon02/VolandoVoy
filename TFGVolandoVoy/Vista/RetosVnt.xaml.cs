using Supabase.Interfaces;
using TFGVolandoVoy.Modelo;

namespace TFGVolandoVoy;

public partial class RetosVnt : ContentPage
{
    private readonly Supabase.Client _supabaseClient;
    private Supabase.Client cliente = new Supabase.Client(ConexionSupabase.SUPABASE_URL, ConexionSupabase.SUPABASE_KEY);

    public RetosVnt()
	{
		InitializeComponent();
        pruebaCargar();
    }

    private async void pruebaCargar()
    {
        await CargarLocalidades();
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
        var response = await cliente.From<Reto>().Where(r => r.IdLocalidad == idLocalidad).Get();
        var retos = response.Models;

        lista_de_retos.ItemsSource = retos;
    }

    private async Task CargarLocalidades()
    {
        

        var response = await cliente.From<Localidad>().Get();
        var localidades = response.Models;

        selector_ciudades.ItemsSource = localidades;
    }

}