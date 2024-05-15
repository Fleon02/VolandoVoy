using Microsoft.Maps.MapControl.WPF.Overlays;
using Supabase;
using Supabase.Interfaces;
using System.Collections.ObjectModel;
using TFGVolandoVoy.Modelo;

namespace TFGVolandoVoy.Vista;

public partial class Retos : ContentPage
{
    private readonly Supabase.Client _supabaseClient;
    private long localidad;
    public string nombreLocalidad;
    

    public string TextoReto { get; set; }

    public Retos(long idLocalidad, Supabase.Client supabaseClient)
    {        
        this.localidad = idLocalidad;
        
        _supabaseClient = supabaseClient;
        ListaRetos = new CollectionView();
        InitializeComponent();
        GetLocalidadById(idLocalidad);
        CargarRetos(idLocalidad);
        
    }
    

    protected override void OnAppearing()
    {
        base.OnAppearing();        
        //CargarRetos();
    }
    

    private async void GetLocalidadById(long idLocalidad)
    {
        try
        {
            
            var localidades = await _supabaseClient.From<Localidad>().Get();

            // Filtrar la lista para encontrar la localidad con el ID específico
            var localidad = localidades.Models.FirstOrDefault(l => l.IdLocalidad == idLocalidad);

            if (localidad != null)
            {
                this.BindingContext = localidad;
            }
            else
            {
                throw new Exception($"No se encontró ninguna localidad con el ID {idLocalidad}");
            }
        }
        catch (Exception ex)
        {
            // Manejar cualquier error que ocurra durante la consulta
            throw new Exception($"Error al obtener la localidad con ID {idLocalidad}: {ex.Message}");
        }
    }

    private async void obtenNombre(long idLocalidad)
    {
        try
        {
            
        }
        catch (Exception ex)
        {

        }
    }

    

    private async void CargarRetos(long idLocalidad)
    {
        try
        {
            // Obtener la lista de retos desde la base de datos
            var retos = await _supabaseClient.From<Reto>().Get();

            if (retos != null)
            {
                // Filtrar la lista de retos para obtener solo los que tienen el ID de localidad específico
                var retosFiltrados = retos.Models.Where(r => r.IdLocalidad == idLocalidad).ToList();

                // Asignar la lista filtrada de retos al ItemSource de tu control ListaRetos
                ListaRetos.ItemsSource = new ObservableCollection<Reto>(retosFiltrados);
            }
            else
            {
                // Manejar el caso en que retos sea null, por ejemplo, mostrar un mensaje de error
                await DisplayAlert("Error", "La lista de retos es null", "Aceptar");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error al cargar los retos: {ex.GetType().FullName}\n{ex.Message}\nStackTrace: {ex.StackTrace}", "Aceptar");
        }
    }

    



}