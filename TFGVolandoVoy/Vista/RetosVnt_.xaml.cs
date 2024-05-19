using Microsoft.Maps.MapControl.WPF.Overlays;
using Supabase;
using Supabase.Interfaces;
using System.Collections.ObjectModel;
using TFGVolandoVoy.Modelo;

namespace TFGVolandoVoy.Vista
{
    public partial class Retos : ContentPage
    {
        private long localidad;
        private RetoViewModel _viewModel;

        public Retos(long idLocalidad)
        {
            this.localidad = idLocalidad;
            _viewModel = new RetoViewModel();
            BindingContext = _viewModel;
            InitializeComponent();
            GetLocalidadById(idLocalidad);
            CargarRetos(idLocalidad);
            Shell.SetFlyoutBehavior(this, FlyoutBehavior.Disabled);
        }

        private async void GetLocalidadById(long idLocalidad)
        {
            try
            {
                Supabase.Client cliente = new Supabase.Client(ConexionSupabase.SUPABASE_URL, ConexionSupabase.SUPABASE_KEY);
                var localidades = await cliente.From<Localidad>().Get();
                var localidad = localidades.Models.FirstOrDefault(l => l.IdLocalidad == idLocalidad);

                if (localidad != null)
                {
                    _viewModel.Localidad = localidad;
                }
                else
                {
                    throw new Exception($"No se encontr� ninguna localidad con el ID {idLocalidad}");
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
                Supabase.Client cliente = new Supabase.Client(ConexionSupabase.SUPABASE_URL, ConexionSupabase.SUPABASE_KEY);
                var retos = await cliente.From<Reto>().Get();
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
