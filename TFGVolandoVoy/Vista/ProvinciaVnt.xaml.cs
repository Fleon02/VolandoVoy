using Microsoft.Maui.Controls;
using System;
using System.IO;
using System.Threading.Tasks;
using TFGVolandoVoy.Modelo;

namespace TFGVolandoVoy
{
    public partial class ProvinciaVnt : ContentPage
    {
        private readonly Supabase.Client _supabaseClient;

        // Constructor con par�metro
        public ProvinciaVnt(Supabase.Client supabaseClient)
        {
            _supabaseClient = supabaseClient;
            InitializeComponent();
        }

        // Constructor sin par�metros
        public ProvinciaVnt() : this(new Supabase.Client(ConexionSupabase.SUPABASE_URL, ConexionSupabase.SUPABASE_KEY))
        {
        }

        // Resto del c�digo de la clase
        private int count = 0;



        private async void OnProvinviaClicked(object sender, EventArgs e)
        {
            // Solicitar los datos de la provincia al usuario
            var nombre = await DisplayPromptAsync("Nombre de la provincia", "Por favor ingrese el nombre de la provincia:");
            var comunidad = await DisplayPromptAsync("Comunidad Aut�noma", "Por favor ingrese la comunidad aut�noma:");
            var imagen = await PickAndUploadImage();

            // Verificar que se hayan proporcionado todos los datos
            if (!string.IsNullOrEmpty(nombre) && !string.IsNullOrEmpty(comunidad) && !string.IsNullOrEmpty(imagen))
            {
                // Crear una nueva instancia de Provincia con los datos ingresados
                Provincia p = new Provincia
                {
                    NombreProvincia = nombre,
                    ComunidadAutonoma = comunidad,
                    ImagenProvincia = imagen
                };

                // Insertar la provincia en la base de datos
                await InsertProvincia(p);
            }
            else
            {
                // Mostrar un mensaje de error si alg�n campo est� vac�o
                await DisplayAlert("Error", "Por favor complete todos los campos obligatorios.", "Aceptar");
            }
        }

        // M�todo para seleccionar y subir una imagen desde el dispositivo del usuario
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
                    var fileName = $"provincia_{DateTime.Now.Ticks}.png"; // Nombre �nico para el archivo
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

            return "null";
        }

        // M�todo para leer el contenido del archivo como un arreglo de bytes
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

        // M�todo para insertar una provincia en la base de datos
        private async Task InsertProvincia(Provincia provincia)
        {
            try
            {
                await _supabaseClient.From<Provincia>().Insert(provincia);
                await DisplayAlert("�xito", "Provincia insertada correctamente.", "Aceptar");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"No se pudo insertar la provincia: {ex.Message}", "Aceptar");
            }
        }


        private async void MostrarClicked(object sender, EventArgs e)
        {
            // Solicitar el ID de la provincia al usuario
            var provinciaId = await DisplayPromptAsync("ID de la provincia", "Por favor ingrese el ID de la provincia:");

            if (!string.IsNullOrEmpty(provinciaId))
            {
                try
                {
                    // Obtener la provincia desde la base de datos usando el ID proporcionado
                    var provincia = await GetProvinciaById(provinciaId);

                    if (provincia != null)
                    {
                        // Construir el mensaje de alerta con los datos de la provincia
                        var message = $"Nombre: {provincia.NombreProvincia}\n" +
                                      $"Comunidad Aut�noma: {provincia.ComunidadAutonoma}\n" +
                                      $"URL de la imagen: {provincia.ImagenProvincia}";

                        // Mostrar el mensaje de alerta
                        Logo.Source = provincia.ImagenProvincia;
                        await DisplayAlert("Detalle de la Provincia", message, "Aceptar");

                    }
                    else
                    {
                        await DisplayAlert("Error", "No se encontr� ninguna provincia con ese ID.", "Aceptar");
                    }
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", $"Ocurri� un error al obtener los datos de la provincia: {ex.Message}", "Aceptar");
                }
            }
            else
            {
                await DisplayAlert("Error", "Por favor ingrese un ID de provincia v�lido.", "Aceptar");
            }
        }


        private async Task<Provincia> GetProvinciaById(string id)
        {
            try
            {
                // Obtener todas las provincias de la base de datos
                var provincias = await _supabaseClient.From<Provincia>().Get();

                // Filtrar la lista para encontrar la provincia con el ID espec�fico
                var provincia = provincias.Models.FirstOrDefault(p => p.IdProvincia == long.Parse(id));

                if (provincia != null)
                {
                    return provincia;
                }
                else
                {
                    throw new Exception($"No se encontr� ninguna provincia con el ID {id}");
                }
            }
            catch (Exception ex)
            {
                // Manejar cualquier error que ocurra durante la consulta
                throw new Exception($"Error al obtener la provincia con ID {id}: {ex.Message}");
            }
        }

    }
}
