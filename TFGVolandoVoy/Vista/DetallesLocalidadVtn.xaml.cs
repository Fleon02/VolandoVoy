using TFGVolandoVoy.Modelo;

namespace TFGVolandoVoy
{
    public partial class DetallesLocalidadVtn : ContentPage
    {
        private readonly Supabase.Client _supabaseClient;

        // Constructor con parámetro
        public DetallesLocalidadVtn(Supabase.Client supabaseClient)
        {
            _supabaseClient = supabaseClient;
            InitializeComponent();
            Shell.SetFlyoutBehavior(this, FlyoutBehavior.Disabled);
        }

        // Constructor sin parámetros
        public DetallesLocalidadVtn(string labelText) : this(new Supabase.Client(ConexionSupabase.SUPABASE_URL, ConexionSupabase.SUPABASE_KEY))
        {

            NombreLocalidad.Text = labelText;
            GetLocalidadByNombreLocalidad(labelText);
        }

        // Método que se llama cada vez que la página se muestra en pantalla
        protected override void OnAppearing()
        {
            base.OnAppearing();
        }
        private async Task<Localidad> GetLocalidadByNombreLocalidad(string NombreLocalidad)
        {
            try
            {
                var localidades = await _supabaseClient.From<Localidad>().Get();
                var localidad = localidades.Models.FirstOrDefault(p => p.NombreLocalidad == NombreLocalidad);
                if (localidad != null)
                {
                    var provincia = await GetProvinciaById(localidad.IdProvincia.ToString());
                    if (provincia != null)
                    {
                        ImgLoc.Source = localidad.ImagenLocalidad;
                        NombreProvincia.Text = $"Provincia: {provincia.NombreProvincia}";
                        ComunidadAutonoma.Text = $"Comunidad Autonoma: {provincia.ComunidadAutonoma}";
                        ComAu.Source = provincia.ImagenProvincia;
                    }
                    else
                    {
                        await DisplayAlert("Error", "No se encontró la provincia asociada a esta localidad.", "Aceptar");
                    }
                    return localidad;
                }
                else
                {
                    throw new Exception($"No se encontró ninguna imagen de {NombreLocalidad}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener {NombreLocalidad}: {ex.Message}");
            }
        }
        private async Task<Provincia> GetProvinciaById(string id)
        {
            try
            {
                var provincias = await _supabaseClient.From<Provincia>().Get();
                var provincia = provincias.Models.FirstOrDefault(p => p.IdProvincia == long.Parse(id));
                if (provincia != null)
                {
                    return provincia;
                }
                else
                {
                    throw new Exception($"No se encontró ninguna provincia con el ID {id}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener la provincia con ID {id}: {ex.Message}");
            }
        }
    }
}
