using Microsoft.Maui.Controls;
using System;
using System.IO;
using System.Threading.Tasks;
using TFGVolandoVoy.Modelo;

namespace TFGVolandoVoy
{
    public partial class LocalidadVnt : ContentPage
    {
        private readonly Supabase.Client _supabaseClient;

        // Constructor con parámetro
        public LocalidadVnt(Supabase.Client supabaseClient)
        {
            _supabaseClient = supabaseClient;
            InitializeComponent();
        }

        // Constructor sin parámetros
        public LocalidadVnt() : this(new Supabase.Client(ConexionSupabase.SUPABASE_URL, ConexionSupabase.SUPABASE_KEY))
        {
        }

        // Método que se llama cada vez que la página se muestra en pantalla
        protected override void OnAppearing()
        {
            base.OnAppearing();

            // Reiniciar la página
            Logo.Source = "logo.png";

            NombreLocalidad.Text = "Localidad : ";
            NombreProvincia.Text = "Provincia : ";
        }

        // Resto del código de la clase
        private int count = 0;



        private async void OnLocalidadClicked(object sender, EventArgs e)
        {
            // Inicializar la variable provincia fuera del bloque try-catch
            Provincia provincia = null;

            try
            {
                // Obtener la lista de provincias
                var provincias = await _supabaseClient.From<Provincia>().Get();

                // Crear una lista de nombres de provincias
                var nombresProvincias = provincias.Models.Select(p => p.NombreProvincia).ToList();

                // Mostrar un diálogo para que el usuario elija la provincia
                var provinciaSeleccionada = await DisplayActionSheet("Seleccione la provincia", "Cancelar", null, nombresProvincias.ToArray());

                if (provinciaSeleccionada != null && provinciaSeleccionada != "Cancelar")
                {
                    // Iterar sobre la lista de provincias y encontrar la que coincide con el nombre seleccionado
                    foreach (var p in provincias.Models)
                    {
                        if (p.NombreProvincia == provinciaSeleccionada)
                        {
                            provincia = p;
                            break;
                        }
                    }

                    // Solicitar los datos de la localidad al usuario
                    var nombre = await DisplayPromptAsync("Nombre de la Localidad", "Por favor ingrese el nombre de la localidad:");
                    var imagen = await PickAndUploadImage();

                    // Verificar que se hayan proporcionado todos los datos
                    if (!string.IsNullOrEmpty(nombre) && !string.IsNullOrEmpty(imagen))
                    {
                        // Crear una nueva instancia de Localidad con los datos ingresados
                        Localidad localidad = new Localidad
                        {
                            NombreLocalidad = nombre,
                            ImagenLocalidad = imagen,
                            IdProvincia = provincia.IdProvincia // Asignar el ID de la provincia seleccionada
                        };

                        // Insertar la localidad en la base de datos
                        await InsertarLocalidad(localidad);
                    }
                    else
                    {
                        // Mostrar un mensaje de error si algún campo está vacío
                        await DisplayAlert("Error", "Por favor complete todos los campos obligatorios.", "Aceptar");
                    }
                }
            }
            catch (Exception ex)
            {
                // Si ocurre alguna excepción, mostrar un mensaje de error
                await DisplayAlert("Error", $"Error al obtener las provincias: {ex.Message}", "Aceptar");
            }

        }


        // Método para seleccionar y subir una imagen desde el dispositivo del usuario
        private async Task<string> PickAndUploadImage()
        {
            // Subir imagen desde el dispositivo del usuario
            var mediaFile = await MediaPicker.PickPhotoAsync();

            if (mediaFile != null)
            {
                // Leer el contenido del archivo como un arreglo de bytes
                byte[] fileBytes = await ReadFileAsBytes(mediaFile);

                if (fileBytes != null)
                {
                    // Subir el archivo al bucket en Supabase
                    var fileName = $"provincia_{DateTime.Now.Ticks}.png"; // Nombre único para el archivo
                    var response = await _supabaseClient.Storage
                        .From("imagenes_Prueba")
                        .Upload(fileBytes, fileName);

                    if (response != null)
                    {
                        // Construir manualmente la URL de la imagen cargada
                        string imageUrl = $"{ConexionSupabase.SUPABASE_URL}/storage/v1/object/public/imagenes_Prueba/{fileName}";
                        return imageUrl;
                    }
                    else
                    {
                        // Mostrar un mensaje de error si la carga de la imagen falla
                        await DisplayAlert("Error", "No se pudo cargar la imagen.", "Aceptar");
                    }
                }
            }

            return null;
        }


        // Método para leer el contenido del archivo como un arreglo de bytes
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

        // Método para insertar una provincia en la base de datos
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
            // Solicitar el ID de la localidad al usuario
            var localidadId = await DisplayPromptAsync("ID de la localidad", "Por favor ingrese el ID de la localidad:");

            if (!string.IsNullOrEmpty(localidadId))
            {
                try
                {
                    // Obtener la localidad desde la base de datos usando el ID proporcionado
                    var localidad = await GetLocalidadById(localidadId);

                    if (localidad != null)
                    {
                        // Obtener la provincia asociada a la localidad
                        var provincia = await GetProvinciaById(localidad.IdProvincia.ToString());

                        if (provincia != null)
                        {
                            Logo.Source = localidad.ImagenLocalidad;
                            // Construir el mensaje de alerta con los nombres de la localidad y la provincia
                            var message = $"Nombre de la Localidad: {localidad.NombreLocalidad}\n" +
                                          $"Provincia: {provincia.NombreProvincia}";

                            // Mostrar el mensaje de alerta
                            await DisplayAlert("Detalle de la Localidad y Provincia", message, "Aceptar");
                            NombreLocalidad.Text = $"Nombre Localidad: {localidad.NombreLocalidad}";
                            NombreProvincia.Text = $"Provincia: {provincia.NombreProvincia}";


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
                // Obtener todas las provincias de la base de datos
                var provincias = await _supabaseClient.From<Provincia>().Get();

                // Filtrar la lista para encontrar la provincia con el ID específico
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
                // Manejar cualquier error que ocurra durante la consulta
                throw new Exception($"Error al obtener la provincia con ID {id}: {ex.Message}");
            }
        }

        private async Task<Localidad> GetLocalidadById(string id)
        {
            try
            {
                // Obtener todas las provincias de la base de datos
                var localidades = await _supabaseClient.From<Localidad>().Get();

                // Filtrar la lista para encontrar la provincia con el ID específico
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
                // Manejar cualquier error que ocurra durante la consulta
                throw new Exception($"Error al obtener la provincia con ID {id}: {ex.Message}");
            }
        }

        private void OnClicLabelTapped(object sender, EventArgs e)
        {
            Navigation.PushAsync(new ProvinciaVnt());
        }

    }
}
