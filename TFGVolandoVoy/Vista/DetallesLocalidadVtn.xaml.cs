using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Text;
using TFGVolandoVoy.Modelo;
using Map = Microsoft.Maui.Controls.Maps.Map;

namespace TFGVolandoVoy
{
    public partial class DetallesLocalidadVtn : ContentPage
    {
        private readonly Supabase.Client _supabaseClient;
        private bool _mostrarTodosLosComentarios = false;

        
        public DetallesLocalidadVtn(Supabase.Client supabaseClient)
        {
            _supabaseClient = supabaseClient;
            InitializeComponent();
            Shell.SetFlyoutBehavior(this, FlyoutBehavior.Disabled);
            
            if (Device.RuntimePlatform == Device.WinUI)
            {
                ImgLoc.HeightRequest = 250;
                ComAu.HeightRequest = 250;
            }
            else if (Device.RuntimePlatform == Device.Android || Device.RuntimePlatform == Device.iOS)
            {
                
                DetailsGrid.RowDefinitions.Clear();
                DetailsGrid.ColumnDefinitions.Clear();
                DetailsGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                DetailsGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                DetailsGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                DetailsGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                DetailsGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

                Grid.SetRow(NombreLocalidad, 0);
                Grid.SetRow(ImgLoc, 1);
                Grid.SetRow(ComAu, 2);
                Grid.SetRow(NombreProvincia, 3);
                Grid.SetRow(ComunidadAutonoma, 4);

                Grid.SetColumn(NombreLocalidad, 0);
                Grid.SetColumnSpan(NombreLocalidad, 2);
                Grid.SetColumn(ImgLoc, 0);
                Grid.SetColumn(ComAu, 0);
                Grid.SetColumn(NombreProvincia, 0);
                Grid.SetColumn(ComunidadAutonoma, 0);
            }
        }

        
        public DetallesLocalidadVtn(string labelText) : this(new Supabase.Client(ConexionSupabase.SUPABASE_URL, ConexionSupabase.SUPABASE_KEY))
        {
            NombreLocalidad.Text = labelText;
            GetLocalidadByNombreLocalidad(labelText);
            CargarComentarios(labelText, false);
        }

        
        protected override void OnAppearing()
        {
            base.OnAppearing();
        }

        
        private async void CargarComentarios(string nombreLocalidad, bool cargarTodos)
        {
            try
            {
                var localidades = await _supabaseClient.From<Localidad>().Get();
                var localidad = localidades.Models.FirstOrDefault(p => p.NombreLocalidad == nombreLocalidad);
                if (localidad != null)
                {
                    var Comentarios = await _supabaseClient.From<Comentarios>().Get();
                    var comentariosLocalidad = Comentarios.Models.Where(l => l.IdLocalidad == localidad.IdLocalidad).ToList();
                    if (comentariosLocalidad.Any())
                    {
                        if (cargarTodos)
                        {
                            
                            var todosComentarios = comentariosLocalidad.OrderByDescending(c => c.Valoracion).ToList();
                            ComentariosListView.ItemsSource = new ObservableCollection<Comentarios>(todosComentarios);
                        }
                        else
                        {
                            
                            var mejoresComentarios = comentariosLocalidad.OrderByDescending(c => c.Valoracion).Take(3).ToList();
                            ComentariosListView.ItemsSource = new ObservableCollection<Comentarios>(mejoresComentarios);
                        }
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

        private async Task<Localidad> GetLocalidadByNombreLocalidad(string nombreLocalidad)
        {
            try
            {
                var localidades = await _supabaseClient.From<Localidad>().Get();
                var localidad = localidades.Models.FirstOrDefault(p => p.NombreLocalidad == nombreLocalidad);
                if (localidad != null)
                {
                    var provincia = await GetProvinciaById(localidad.IdProvincia.ToString());
                    if (provincia != null)
                    {
                        ImgLoc.Source = localidad.ImagenLocalidad;
                        NombreProvincia.Text = $"Provincia: {provincia.NombreProvincia}";
                        ComunidadAutonoma.Text = $"Comunidad Autonoma: {provincia.ComunidadAutonoma}";
                        ComAu.Source = provincia.ImagenProvincia;
                       
                        var lugaresInteres = await _supabaseClient.From<LugarInteres>().Get();
                        var lugaresInteresL = lugaresInteres.Models.Where(l => l.IdLocalidad == localidad.IdLocalidad).ToList();
                        
                        foreach (var lugar in lugaresInteresL)
                        {
                            var lugarLabel = new Label
                            {
                                Text = $"{lugar.Lugar} ({lugar.Tipo})",
                                FontSize = 18,
                                HorizontalTextAlignment = TextAlignment.Center,
                            };
                            SLLugaresInteres.Children.Add(lugarLabel);
                        }
                        MapaLocalidadAsync(localidad, provincia);
                       
                    }
                    else
                    {
                        await DisplayAlert("Error", "No se encontró la provincia asociada a esta localidad.", "Aceptar");
                    }
                    return localidad;
                }
                else
                {
                    throw new Exception($"No se encontró ninguna imagen de {nombreLocalidad}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener {nombreLocalidad}: {ex.Message}");
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

        private async Task MapaLocalidadAsync(Localidad localidad, Provincia provincia)
        {
            var mapView = new Map
            {
                HeightRequest = 300,
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                IsShowingUser = false,
                MapType = MapType.Street
            };
            var coordenadas = new Location(localidad.Coordenada1, localidad.Coordenada2);
            Pin pinmapa = new Pin
            {
                Label = localidad.NombreLocalidad + " (" + provincia.NombreProvincia + ")",          
                Location = coordenadas
            };
            mapView.Pins.Add(pinmapa);
            
            if (Device.RuntimePlatform == Device.WinUI)
            {
                
                var lugaresInteres = await _supabaseClient.From<LugarInteres>().Get();
                var lugaresInteresL = lugaresInteres.Models.Where(l => l.IdLocalidad == localidad.IdLocalidad).ToList();
               
                foreach (var lugares in lugaresInteresL)
                {
                    var coordenadaslugar = new Location(lugares.Latitud, lugares.Longitud);
                    Pin pinmapalugar = new Pin
                    {
                        Label = lugares.Lugar + " (" + lugares.Tipo + ")",             
                        Location = coordenadaslugar
                    };
                    mapView.Pins.Add(pinmapalugar);
                }
            }
            mapView.MoveToRegion(MapSpan.FromCenterAndRadius(coordenadas, Distance.FromKilometers(1)));
            SLMapLocalidad.Children.Add(mapView);
        }

        private void MapaLocalidadBing(Localidad localidad, Provincia provincia)
        {
            MapaLocalidadAsync(localidad, provincia);
        }
        private async Task MostrarLugaresInteres(double lat, double lon)
        {
            string apiKey = "AIzaSyBLmtoFc5WL6HX3g5D0AtVbjnd_8uTmAlU";
            string url = "https://places.googleapis.com/v1/places:searchNearby";
            var requestBody = new
            {
                maxResultCount = 5,
                locationRestriction = new
                {
                    circle = new
                    {
                        center = new { latitude = lat, longitude = lon },
                        radius = 1000.0
                    }
                }
            };

            var jsonContent = JsonConvert.SerializeObject(requestBody);
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("X-Goog-Api-Key", apiKey);
                client.DefaultRequestHeaders.Add("X-Goog-FieldMask", "places.displayName,places.primaryTypeDisplayName");

                var response = await client.PostAsync(url, httpContent);
                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var data = JsonConvert.DeserializeObject<dynamic>(jsonResponse);

                    
                    var results = data.places;
                    string places = "";
                    string types = "";
                    int index = 1;
                    foreach (var result in results)
                    {
                        string name = result["displayName"]["text"].ToString();
                        
                        if (result["primaryTypeDisplayName"] != null)
                        {
                            types = result["primaryTypeDisplayName"]["text"].ToString();
                            places += $"{index}. {name} - Tipo: {types}\n";
                        }
                        else
                        {
                            places += $"{index}. {name} - Tipo: No disponible\n";
                        }

                        index++;
                    }


                }
                else
                {
                    await DisplayAlert("Error", "Error en la solicitud: " + response.ReasonPhrase, "OK");
                }
            }
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
                    if (!int.TryParse(ValoracionNum.Text, out valoracion) || valoracion < 1 || valoracion > 5)
                    {
                        await DisplayAlert("Error", "La valoración debe ser un número entre 1 y 5", "Aceptar");
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
                        ComentarioT.Text = string.Empty; 
                        ValoracionNum.Text = string.Empty; 
                        CargarComentarios(NombreLocalidad.Text, false); 
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

        
        private void CambioComentario(object sender, EventArgs e)
        {
            _mostrarTodosLosComentarios = !_mostrarTodosLosComentarios;

            if (_mostrarTodosLosComentarios)
            {
                CargarComentarios(NombreLocalidad.Text, true); 
                NumComentarios.Text = "Ver Menos Comentarios";
            }
            else
            {
                CargarComentarios(NombreLocalidad.Text, false); 
                NumComentarios.Text = "Ver Más Comentarios";
            }
        }
    }
}
