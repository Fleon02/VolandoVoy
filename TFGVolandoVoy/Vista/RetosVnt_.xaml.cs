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
    

    public string TextoReto { get; set; }

    public Retos(long idLocalidad, Supabase.Client supabaseClient)
    {        
        this.localidad = idLocalidad;
        _supabaseClient = supabaseClient;
        ListaRetos = new CollectionView();
        InitializeComponent();
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
            var retos = await _supabaseClient.From<Reto>().Get();

            if (retos != null)
            {
                // Verificar si retos es null antes de acceder a su propiedad Models
                ListaRetos.ItemsSource = new ObservableCollection<Reto>(retos.Models);
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