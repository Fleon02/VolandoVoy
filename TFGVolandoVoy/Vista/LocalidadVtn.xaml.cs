using System.Collections.ObjectModel;
using TFGVolandoVoy.Modelo;

namespace TFGVolandoVoy
{
    public partial class LocalidadVnt : ContentPage
    {
        private readonly Supabase.Client _supabaseClient;

        
        public LocalidadVnt(Supabase.Client supabaseClient)
        {
            _supabaseClient = supabaseClient;
            InitializeComponent();
        }
        private void OnImageSizeChanged(object sender, EventArgs e)
        {
            
            if (DeviceInfo.Platform == DevicePlatform.WinUI)
            {
                
                var image = (Image)sender;
                image.WidthRequest = 350; 
                image.HeightRequest = 75; 
            }
        }

        
        public LocalidadVnt() : this(new Supabase.Client(ConexionSupabase.SUPABASE_URL, ConexionSupabase.SUPABASE_KEY))
        {
        }

        
        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (AppShell.CurrentUser.Rol == "admin")
            {
                crearLocalidadBoton.IsVisible = true;
            }
            else
            {
                crearLocalidadBoton.IsVisible = false;
            }
           
            Logo.Source = "logo.png";

            CargarLocalidades();
        }
        
        private async void CargarLocalidades()
        {
            try
            {
                
                var localidades = await _supabaseClient.From<Localidad>().Get();
                
                var localidadesOrdenadas = localidades.Models.OrderBy(l => l.NombreLocalidad);
                
                LocalidadesListView.ItemsSource = new ObservableCollection<Localidad>(localidadesOrdenadas);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error al cargar las localidades: {ex.Message}", "Aceptar");
            }
        }

        private int count = 0;
        private async void OnLocalidadClicked(object sender, EventArgs e)
        {
            
            Provincia provincia = null;
            try
            {
               
                var provincias = await _supabaseClient.From<Provincia>().Get();

                
                var nombresProvincias = provincias.Models.Select(p => p.NombreProvincia).ToList();

                
                var provinciaSeleccionada = await DisplayActionSheet("Seleccione la provincia", "Cancelar", null, nombresProvincias.ToArray());

                if (provinciaSeleccionada != null && provinciaSeleccionada != "Cancelar")
                {
                    
                    foreach (var p in provincias.Models)
                    {
                        if (p.NombreProvincia == provinciaSeleccionada)
                        {
                            provincia = p;
                            break;
                        }
                    }

                    
                    var nombre = await DisplayPromptAsync("Nombre de la Localidad", "Por favor ingrese el nombre de la localidad:");
                    var imagen = await ElegirImagen();

                    
                    if (!string.IsNullOrEmpty(nombre) && !string.IsNullOrEmpty(imagen))
                    {
                        
                        Localidad localidad = new Localidad
                        {
                            NombreLocalidad = nombre,
                            ImagenLocalidad = imagen,
                            IdProvincia = provincia.IdProvincia 
                        };

                        
                        await InsertarLocalidad(localidad);
                    }
                    else
                    {
                        
                        await DisplayAlert("Error", "Por favor complete todos los campos obligatorios.", "Aceptar");
                    }
                }
            }
            catch (Exception ex)
            {
                
                await DisplayAlert("Error", $"Error al obtener las provincias: {ex.Message}", "Aceptar");
            }

        }


        private async Task<string> ElegirImagen()
        {
            
            var mediaFile = await MediaPicker.PickPhotoAsync();

            if (mediaFile != null)
            {
                
                byte[] fileBytes = await LeerArchivo(mediaFile);

                if (fileBytes != null)
                {
                    
                    var fileName = $"provincia_{DateTime.Now.Ticks}.png"; 
                    var response = await _supabaseClient.Storage
                        .From("imagenes_Prueba")
                        .Upload(fileBytes, fileName);

                    if (response != null)
                    {
                        
                        string imageUrl = $"{ConexionSupabase.SUPABASE_URL}/storage/v1/object/public/imagenes_Prueba/{fileName}";
                        return imageUrl;
                    }
                    else
                    {
                        
                        await DisplayAlert("Error", "No se pudo cargar la imagen.", "Aceptar");
                    }
                }
            }

            return null;
        }


        
        private async Task<byte[]> LeerArchivo(FileResult file)
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

        
        private async Task InsertarLocalidad(Localidad localidad)
        {
            try
            {
                await _supabaseClient.From<Localidad>().Insert(localidad);
                await DisplayAlert("Éxito", "Localidad insertada correctamente.", "Aceptar");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"No se pudo insertar la localidad: {ex.Message}", "Aceptar");
            }
        }

        private async void MostrarLocalidades(object sender, EventArgs e)
        {
            
            var localidadId = await DisplayPromptAsync("ID de la localidad", "Por favor ingrese el ID de la localidad:");

            if (!string.IsNullOrEmpty(localidadId))
            {
                try
                {
                    
                    var localidad = await GetLocalidadById(localidadId);
                    if (localidad != null)
                    {
                        
                        var provincia = await GetProvinciaById(localidad.IdProvincia.ToString());
                        if (provincia != null)
                        {
                            Logo.Source = localidad.ImagenLocalidad;
                            
                            var message = $"Nombre de la Localidad: {localidad.NombreLocalidad}\n" +
                                          $"Provincia: {provincia.NombreProvincia}";
                            
                            await DisplayAlert("Detalle de la Localidad y Provincia", message, "Aceptar");

                        }
                        else
                        {
                            await DisplayAlert("Error", "No se encontró la provincia asociada a esta localidad.", "Aceptar");
                        }
                    }
                    else
                    {
                        await DisplayAlert("Error", "No se encontró ninguna localidad con ese ID.", "Aceptar");
                    }
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", $"Ocurrió un error al obtener los datos de la localidad: {ex.Message}", "Aceptar");
                }
            }
            else
            {
                await DisplayAlert("Error", "Por favor ingrese un ID de localidad válido.", "Aceptar");
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

        private async Task<Localidad> GetLocalidadById(string id)
        {
            try
            {
                
                var localidades = await _supabaseClient.From<Localidad>().Get();

                
                var localidad = localidades.Models.FirstOrDefault(p => p.IdLocalidad == long.Parse(id));

                if (localidad != null)
                {
                    return localidad;
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

        private void DetallesL(object sender, EventArgs e)
        {
            var label = (Label)sender;
            var labelText = label.Text;
            Navigation.PushAsync(new DetallesLocalidadVtn(labelText));
        }

        private void crearLocalidadBoton_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new CrearLocalidad());
        }
    }
}
