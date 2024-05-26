using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using System.Collections.ObjectModel;
using TFGVolandoVoy.Modelo;
using TFGVolandoVoy.Vista;
using Map = Microsoft.Maui.Controls.Maps.Map;

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
            CargarComentarios(labelText);
        }

        // Método que se llama cada vez que la página se muestra en pantalla
        protected override void OnAppearing()
        {
            base.OnAppearing();
        }
        // Método para cargar la lista de Comentarios
        private async void CargarComentarios(string NombreLocalidad)
        {
            try
            {
                var localidades = await _supabaseClient.From<Localidad>().Get();
                var localidad = localidades.Models.FirstOrDefault(p => p.NombreLocalidad == NombreLocalidad);
                if (localidad != null)
                {
                    var Comentarios = await _supabaseClient.From<Comentarios>().Get();
                    var comentariosLocalidad = Comentarios.Models.Where(l => l.IdLocalidad == localidad.IdLocalidad).ToList();
                    if (comentariosLocalidad.Any())
                    {
                        // Ordenar los comentarios por valoración (de mayor a menor)
                        var mejoresComentarios = comentariosLocalidad.OrderByDescending(c => c.Valoracion).Take(3).ToList();
                        ComentariosListView.ItemsSource = new ObservableCollection<Comentarios>(mejoresComentarios);
                    }
                }
                else
                {
                    await DisplayAlert("Advertencia", "Localidad no encontrada", "Aceptar");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error al cargar los comentarios: {ex.Message}", "Aceptar");
            }
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
                        MapaLocalidad(localidad, provincia);
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

        private void MapaLocalidad(Localidad localidad, Provincia provincia)
        {
            var mapView = new Map
            {
                HeightRequest = 200,
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                IsShowingUser = false,
                MapType = MapType.Street
            };
            var coordenadas = new Location(localidad.Coordenada1, localidad.Coordenada2);
            Pin pinmapa = new Pin
            {
                Label = localidad.NombreLocalidad + " (" + provincia.NombreProvincia + ")", // Nombre del marcador            
                Location = coordenadas
            };
            mapView.Pins.Add(pinmapa);

            mapView.MoveToRegion(MapSpan.FromCenterAndRadius(coordenadas, Distance.FromKilometers(1)));
            stacklayout.Children.Add(mapView);
        }
        private void MapaLocalidadBing(Localidad localidad, Provincia provincia)
        {
            MapaLocalidad(localidad, provincia);
        }
        private async void CrearComentario(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ComentarioT.Text))
            {
                await DisplayAlert("Error", "El comentario no puede estar vacío", "Aceptar");
                return;
            }
            try
            {
                var localidades = await _supabaseClient.From<Localidad>().Get();
                var localidad = localidades.Models.FirstOrDefault(p => p.NombreLocalidad == NombreLocalidad.Text);
                if (localidad != null)
                {
                    var currentUser = AppShell.CurrentUser;
                    var usuarios = await _supabaseClient.From<Usuario>().Get();
                    var usuario = usuarios.Models.FirstOrDefault(u => u.NombreUsuario == currentUser.Username);
                    if (usuario == null)
                    {
                        await DisplayAlert("Error", "Usuario no encontrado", "Aceptar");
                        return;
                    }
                    int valoracion;
                    if (!int.TryParse(ValoracionNum.Text, out valoracion) || valoracion < 1 || valoracion > 10)
                    {
                        await DisplayAlert("Error", "La valoración debe ser un número entre 1 y 10", "Aceptar");
                        return;
                    }
                    var nuevoComentario = new Comentarios
                    {
                        IdUsuario = usuario.IdUsuario,
                        IdLocalidad = localidad.IdLocalidad,
                        Comentario = ComentarioT.Text,
                        Valoracion = valoracion
                    };
                    var resultado = await _supabaseClient.From<Comentarios>().Insert(nuevoComentario);
                    if (resultado != null)
                    {
                        await DisplayAlert("Éxito", "Comentario insertado correctamente", "Aceptar");
                        ComentarioT.Text = string.Empty; // Limpiar campo de comentario
                        ValoracionNum.Text = string.Empty; // Restablecer campo de valoración
                        CargarComentarios(NombreLocalidad.Text); // Recargar comentarios
                    }
                    else
                    {
                        await DisplayAlert("Error", "Error al insertar el comentario", "Aceptar");
                    }
                }
                else
                {
                    await DisplayAlert("Error", "Localidad no encontrada", "Aceptar");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error al insertar el comentario: {ex.Message}", "Aceptar");
            }
        }
    }
}