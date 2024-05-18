using Microsoft.Maps.MapControl.WPF.Overlays;
using Supabase;
using Supabase.Interfaces;
using System.Collections.ObjectModel;
using TFGVolandoVoy.Modelo;

namespace TFGVolandoVoy.Vista
{
    public partial class Retos : ContentPage
    {
        private readonly Supabase.Client _supabaseClient;
        private long localidad;
        private RetoViewModel _viewModel;

        public Retos(long idLocalidad, Supabase.Client supabaseClient)
        {
            this.localidad = idLocalidad;
            _supabaseClient = supabaseClient;
            _viewModel = new RetoViewModel();
            BindingContext = _viewModel;
            InitializeComponent();
            GetLocalidadById(idLocalidad);
            CargarRetos(idLocalidad);
        }

        private async void GetLocalidadById(long idLocalidad)
        {
            try
            {
                var localidades = await _supabaseClient.From<Localidad>().Get();
                var localidad = localidades.Models.FirstOrDefault(l => l.IdLocalidad == idLocalidad);

                if (localidad != null)
                {
                    _viewModel.Localidad = localidad;
                }
                else
                {
                    throw new Exception($"No se encontró ninguna localidad con el ID {idLocalidad}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener la localidad con ID {idLocalidad}: {ex.Message}");
            }
        }

        private async void CargarRetos(long idLocalidad)
        {
            try
            {
                var retos = await _supabaseClient.From<Reto>().Get();
                if (retos != null)
                {
                    var retosFiltrados = retos.Models.Where(r => r.IdLocalidad == idLocalidad).ToList();
                    if (retosFiltrados.Any())
                    {
                        _viewModel.TipoDeReto = retosFiltrados.First().TipoDeReto;
                    }
                    ListaRetos.ItemsSource = new ObservableCollection<Reto>(retosFiltrados);
                }
                else
                {
                    await DisplayAlert("Error", "La lista de retos es null", "Aceptar");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error al cargar los retos: {ex.GetType().FullName}\n{ex.Message}\nStackTrace: {ex.StackTrace}", "Aceptar");
            }
        }
    }
}
