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
        String? imagenElegida = null;




        public CrearReto(Supabase.Client supabaseClient)
        {
            InitializeComponent();
            _supabaseClient = supabaseClient;
            Localidades = new ObservableCollection<Localidad>();
            BindingContext = this;
            Shell.SetFlyoutBehavior(this, FlyoutBehavior.Disabled);
            var temaActual = App.Current.RequestedTheme;

            // Establecer la imagen según el tema
            if (temaActual != AppTheme.Dark)
            {
                cargarImagenRetoPreviewBtn.Source = "select_imagedark.png";
            }
            else
            {
                cargarImagenRetoPreviewBtn.Source = "select_image.png";
            }
            CargarLocalidades();
        }

        public CrearReto() : this(new Supabase.Client(ConexionSupabase.SUPABASE_URL, ConexionSupabase.SUPABASE_KEY))
        {
        }



        private async Task CargarLocalidades()
        {
            try
            {
                var response = await _supabaseClient.From<Localidad>().Get();
                var localidades = response.Models;

                if (localidades != null && localidades.Count > 0)
                {
                    Localidades.Clear();
                    var localidadesOrdenadas = localidades.OrderBy(l => l.NombreLocalidad).ToList();
                    foreach (var localidad in localidadesOrdenadas)
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



        
        private void Selector_ciudades_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (selector_ciudades.SelectedIndex != -1)
            {
                var localidadSeleccionada = (Localidad)selector_ciudades.SelectedItem;
                // Manejar la localidad seleccionada si es necesario
            }
        }




        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await CargarLocalidades();
        }




        private async void InsertarReto_Clicked(object sender, EventArgs e)
        {
            if (LocalidadSeleccionada == null)
            {
                await DisplayAlert("Error", "Seleccione una localidad.", "OK");
                return;
            }

            long idLocalidad = LocalidadSeleccionada.IdLocalidad;
            string descripcionReto = RetoDefinicion.Text;
            string tipoReto = TipoReto.Text;

            var nuevoReto = new Reto
            {
                IdLocalidad = idLocalidad,
                DescripcionReto = descripcionReto,
                Superado = false,
                TipoDeReto = tipoReto,
                ImagenRetoPreview = imagenElegida,
                ImagenCompletado = "https://clfynwobrskueprtvnmg.supabase.co/storage/v1/object/public/ImagenesRetoCompletado/reto_no_completado.png"
            };

            try
            {
                var insertTask = await _supabaseClient.From<Reto>().Insert(nuevoReto);

                // Verificar si la inserción fue exitosa
                if (insertTask != null)
                {
                    // Inserción exitosa
                    await DisplayAlert("Éxito", "El reto se ha insertado correctamente.", "OK");

                    // Limpiar los campos después de la inserción exitosa
                    RetoDefinicion.Text = "";
                    TipoReto.Text = "";
                    selector_ciudades.SelectedItem = null;
                }
                else
                {
                    // Error al insertar el reto
                    await DisplayAlert("Error", "No se pudo insertar el reto. Por favor, inténtelo de nuevo.", "OK");
                }
            }
            catch (Exception ex)
            {
                // Manejar el error
                await DisplayAlert("Error", $"Ocurrió un error: {ex.Message}", "OK");
            }
        }

        private async void Imagen_antes_reto(object sender, EventArgs e)
        {

            imagenElegida = await SeleccionarImagen();
        }

        private async Task<string> SeleccionarImagen()
        {
            var mediaFile = await MediaPicker.PickPhotoAsync();

            if (mediaFile != null)
            {
                byte[] fileBytes = await ReadFileAsBytes(mediaFile);

                if (fileBytes != null)
                {
                    var fileName = $"imgRetoPreview_{DateTime.Now.Ticks}.png";
                    var response = await _supabaseClient.Storage.From("ImagenesRetosPreview").Upload(fileBytes, fileName);

                    if (response != null)
                    {
                        string imageUrl = $"{ConexionSupabase.SUPABASE_URL}/storage/v1/object/public/ImagenesRetosPreview/{fileName}";
                        return imageUrl;
                    }
                    else
                    {
                        await DisplayAlert("Error", "No se pudo cargar la imagen.", "Aceptar");
                    }
                }
            }

            return "null";
        }

        private async Task<byte[]> ReadFileAsBytes(FileResult file)
        {
            using (var stream = await file.OpenReadAsync())
            {
                using (var memoryStream = new MemoryStream())
                {
                    await stream.CopyToAsync(memoryStream);
                    return memoryStream.ToArray();
                }
            }
        }
    }
}