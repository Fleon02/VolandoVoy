using Supabase.Interfaces;
using Microsoft.Maui.Controls;
using TFGVolandoVoy.Modelo;
using Supabase.Realtime;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace TFGVolandoVoy.Vista
{
    public partial class CrearReto : ContentPage
    {
        public ObservableCollection<Localidad> Localidades { get; set; }
        public Localidad LocalidadSeleccionada { get; set; }
        private readonly Supabase.Client _supabaseClient;

        public CrearReto(Supabase.Client supabaseClient)
        {
            InitializeComponent();
            _supabaseClient = supabaseClient;
            Localidades = new ObservableCollection<Localidad>();
            BindingContext = this;  // Establecer el contexto de enlace

            // Cargar localidades al aparecer la vista
            CargarLocalidades();
        }

        private async void CargarLocalidades()
        {
            try
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
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cargar localidades: {ex.Message}");
            }
        }

        // Opcional: M�todo para manejar el evento cuando se selecciona una localidad
        private void Selector_ciudades_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (selector_ciudades.SelectedIndex != -1)
            {
                var localidadSeleccionada = (Localidad)selector_ciudades.SelectedItem;
                // Manejar la localidad seleccionada si es necesario
            }
        }
    }
}


    //protected override async void OnAppearing()
    //{
    //    base.OnAppearing();
    //    await CargarLocalidades();
    //}

    //public async void InsertarReto_Clicked(object sender, EventArgs e)
    //{
    //    Localidad localidadSeleccionada = (Localidad)selector_ciudades.SelectedItem;
    //    long idLocalidad = localidadSeleccionada.IdLocalidad;
    //    string descripcionReto = RetoDefinicion.Text;
    //    string tipoReto = TipoReto.Text;


    //    var nuevoReto = new Reto
    //    {
    //        IdLocalidad = idLocalidad,
    //        DescripcionReto = descripcionReto,
    //        Superado = false,
    //        TipoDeReto = tipoReto
    //    };

    //    var insertTask = _supabaseClient.From<Reto>().Insert(nuevoReto);

    //    // Esperar a que se complete la operaci�n de inserci�n
    //    await insertTask;

    //    // Verificar si la inserci�n fue exitosa
    //    if (insertTask.IsCompletedSuccessfully)
    //    {
    //        // Inserci�n exitosa
    //        await DisplayAlert("�xito", "El reto se ha insertado correctamente.", "OK");

    //        // Limpiar los campos despu�s de la inserci�n exitosa
    //        RetoDefinicion.Text = "";
    //        TipoReto.Text = "";
    //    }
    //    else
    //    {
    //        // Error al insertar el reto
    //        await DisplayAlert("Error", "No se pudo insertar el reto. Por favor, int�ntelo de nuevo.", "OK");
    //    }
    //}

    //private void selector_ciudades_SelectedIndexChanged(object sender, EventArgs e)
    //{
    //    if (selector_ciudades.SelectedIndex != -1)
    //    {
    //        var localidadSeleccionada = (Localidad)selector_ciudades.SelectedItem;
    //        long idLocalidad = localidadSeleccionada.IdLocalidad;
    //        string nombreLocalidad = localidadSeleccionada.NombreLocalidad;

    //    }
    //}


    //private void Selector_ciudades_SelectedIndexChanged(object sender, EventArgs e)
    //{
    //    if (selector_ciudades.SelectedIndex != -1)
    //    {
    //        var localidadSeleccionada = (Localidad)selector_ciudades.SelectedItem;
    //        long idLocalidad = localidadSeleccionada.IdLocalidad;
    //        string nombreLocalidad = localidadSeleccionada.NombreLocalidad;

    //    }
    //}
