using Supabase.Interfaces;
using System.Collections.ObjectModel;
using TFGVolandoVoy.Modelo;
using TFGVolandoVoy.Vista;

namespace TFGVolandoVoy;

public partial class RetosVnt : ContentPage
{
    private readonly Supabase.Client _supabaseClient;
    public ObservableCollection<Localidad> Localidades { get; set; }
    public ObservableCollection<Reto> Retos { get; set; }





    
    public RetosVnt(Supabase.Client supabaseClient)
    {
        _supabaseClient = supabaseClient;
        InitializeComponent();
        _ = CargarLocalidades();
        Localidades = new ObservableCollection<Localidad>();
        Retos = new ObservableCollection<Reto>();
        selector_ciudades.ItemsSource = Localidades;
        RetosListView.ItemsSource = Retos;
        this.BindingContext = this;
        if (AppShell.CurrentUser.Rol == "admin")
        {
            crear_retos_boton.IsVisible = true;
        }
        else
        {
            crear_retos_boton.IsVisible = false;
        }


    }




    
    public RetosVnt() : this(new Supabase.Client(ConexionSupabase.SUPABASE_URL, ConexionSupabase.SUPABASE_KEY))
    {
    }




    private async void selector_ciudades_SelectedIndexChanged(object sender, EventArgs e)
    {
        Retos.Clear();
        var selectedCiudad = selector_ciudades.SelectedItem as Localidad;
        if (selectedCiudad != null)
        {
            await CargarRetos(selectedCiudad);

        }
    }




    private async Task CargarRetos(Localidad localidad)
    {
        var response = await _supabaseClient.From<Reto>().Where(x => x.IdLocalidad == localidad.IdLocalidad).Get();
        var retos = response.Models;

        if (retos != null && retos.Count > 0)
        {
            Retos.Clear();
            foreach (var reto in retos)
            {
                Retos.Add(reto);
                
            }

        }
        else
        {
           await DisplayAlert("Error", "No existen Retos para esta localidad", "Aceptar");

        }
    }





    private async Task ActualizarRetos(long idLocalidad)
    {
        var response = await _supabaseClient.From<Reto>().Where(r => r.IdLocalidad == idLocalidad).Get();
        var retos = response.Models;

        RetosListView.ItemsSource = retos;
    }




    private async Task CargarLocalidades()
    {
        var response = await _supabaseClient.From<Localidad>().Get();
        var localidades = response.Models;

        if (localidades != null && localidades.Count > 0)
        {
            Localidades.Clear();
            foreach (var localidad in localidades)
            {
                Localidades.Add(localidad);
            }
        }
        else
        {
            Console.WriteLine("No se encontraron localidades.");
        }

        selector_ciudades.ItemsSource = localidades;
    }



    protected override async void OnAppearing()
    {
        base.OnAppearing();

        
        Localidades.Clear();
        Retos.Clear();
        selector_ciudades.SelectedIndex = -1;

        await CargarLocalidades();
    }

    private void crear_retos_boton_Clicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new CrearReto());
    }

    private void DetallesR(object sender, TappedEventArgs e)
    {
        var label = (Label)sender;
        var labelText = label.Text;
        Navigation.PushAsync(new DetallesReto(labelText));
    }

}