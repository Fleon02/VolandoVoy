using Supabase.Interfaces;

namespace TFGVolandoVoy.Vista;

public partial class RetosVnt : ContentPage
{
    private readonly Supabase.Client _supabaseClient;

    public RetosVnt()
	{
		InitializeComponent();
        CargarLocalidades();
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

        selector_ciudades.ItemsSource = localidades;
    }

}