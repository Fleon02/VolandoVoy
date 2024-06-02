using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Text;
using TFGVolandoVoy.Modelo;
using static System.Net.WebRequestMethods;

namespace TFGVolandoVoy;

public partial class CrearLocalidad : ContentPage
{
    private const string ApiKey = "AIzaSyBLmtoFc5WL6HX3g5D0AtVbjnd_8uTmAlU";
    private const string AutocompleteUrl = "https://places.googleapis.com/v1/places:autocomplete";
    private List<Place> suggestions; // Store suggestions with place name and ID

    private string nombreLocalidad = "";
    private string imgLocalidad = "";
    private long idProvincia;
    private string nombreProvincia = "";
    private double longitud;
    private double latitud;
    private string imgProvincia = "";

    Dictionary<string, string> lugaresInteres;

    public CrearLocalidad()
	{
		InitializeComponent();

        Shell.SetFlyoutBehavior(this, FlyoutBehavior.Disabled);

        suggestions = new List<Place>(); // Initialize empty suggestion list

        lugaresInteres = new Dictionary<string, string>();


    }

    private async void textBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
    {
        string input = e.NewTextValue;
        if (input.Length >= 3)
        {
            suggestions = await GetAutocompleteSuggestionsAsync(input);
            listBoxPlaces.ItemsSource = null;
            listBoxPlaces.ItemsSource = suggestions;
        }
        else
        {
            listBoxPlaces.ItemsSource = null;
        }
    }

    private async Task<List<Place>> GetAutocompleteSuggestionsAsync(string input)
    {
        using (var client = new HttpClient())
        {
            var requestBody = new
            {
                input = input,
                languageCode = "es",
                includedRegionCodes = "es",
                includedPrimaryTypes = "locality"
            };

            var jsonContent = JsonConvert.SerializeObject(requestBody);
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            client.DefaultRequestHeaders.Add("X-Goog-Api-Key", ApiKey);

            var response = await client.PostAsync(AutocompleteUrl, httpContent);
            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<dynamic>(jsonResponse);
                var suggestionsRaw = data.suggestions;

                List<Place> places = new List<Place>();
                if (suggestionsRaw != null)
                {
                    for (int i = 0; i < suggestionsRaw.Count; i++)
                    {
                        var placePrediction = suggestionsRaw[i]?.placePrediction;
                        if (placePrediction != null)
                        {
                            var placeDescription = placePrediction.text.text?.ToString();
                            var placeId = placePrediction.placeId?.ToString();
                            if (!string.IsNullOrEmpty(placeDescription) && !string.IsNullOrEmpty(placeId))
                            {
                                places.Add(new Place(placeDescription, placeId));
                            }
                        }
                    }
                }
                return places;
            }
            return new List<Place>();
        }
    }

    private async void ListBoxPlaces_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        if (e.SelectedItem != null)
        {
            var selectedPlace = (Place)e.SelectedItem;
            string placeId = selectedPlace.PlaceId;

            await DisplayAlert("Place Selected", $"{placeId} - {selectedPlace.Description}", "OK");

            GetAddressDetails(placeId);
        }
    }

    private async void GetAddressDetails(string placeId)
    {
        string url = $"https://places.googleapis.com/v1/places/{placeId}?fields=addressComponents,location,photos&key={ApiKey}&languageCode=es";

        using (var client = new HttpClient())
        {
            var response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                dynamic data = JsonConvert.DeserializeObject(json);

                string locality = "";
                string administrativeAreaLevel1 = "";
                string administrativeAreaLevel2 = "";
                string country = "";
                string postalCode = "";
                string PhotosName = "";

                double latitude = 0.0;
                double longitude = 0.0;

                if (data != null)
                {
                    var addressComponents = data.addressComponents;
                    if (data.photos != null && data.photos.Count > 0)
                    {
                        PhotosName = data.photos[0]["name"]?.ToString() ?? "Sin Fotos";
                    }

                    foreach (var component in addressComponents)
                    {
                        var types = component.types as JArray;
                        string longText = component.longText?.ToString();

                        if (types != null && longText != null)
                        {
                            foreach (string type in types)
                            {
                                switch (type)
                                {
                                    case "locality":
                                        locality = longText;
                                        nombreLocalidad = longText;
                                        break;
                                    case "administrative_area_level_2":
                                        administrativeAreaLevel2 = longText;
                                        nombreProvincia = longText;
                                        break;
                                    case "country":
                                        country = longText;
                                        break;
                                    case "postal_code":
                                        postalCode = longText;
                                        break;
                                }
                            }

                            administrativeAreaLevel1 = ProvinciasComunidades.GetComunidadAutonoma(administrativeAreaLevel2);

                        }
                    }

                    var location = data.location;
                    if (location != null)
                    {
                        latitude = Convert.ToDouble(location["latitude"]);
                        longitude = Convert.ToDouble(location["longitude"]);

                        longitud = Convert.ToDouble(location["longitude"]);
                        latitud = Convert.ToDouble(location["latitude"]);
                    }
                }

                string addressDetails = $"Localidad: {locality}\nProvincia: {administrativeAreaLevel2}\nComunidad Autonoma: {administrativeAreaLevel1}\nPaís: {country}\nCódigo Postal: {postalCode}\n\nCoordenadas:\nLatitud: {latitude:F6}\nLongitud: {longitude:F6}";
                
                await DisplayAlert("Detalles de la dirección", addressDetails, "OK");



                await ShowNearbyPlacesAsync(latitude, longitude, placeId, PhotosName);
            }
        }
    }

    private async Task ShowNearbyPlacesAsync(double lat, double lon, string placeId, string photosName)
    {
        string url = "https://places.googleapis.com/v1/places:searchNearby";
        string[] tiposExcluidos = { "subway_station", "train_station", "bank", "supermarket" };
        var requestBody = new
        {
            maxResultCount = 5,
            locationRestriction = new
            {
                circle = new
                {
                    center = new
                    {
                        latitude = lat,
                        longitude = lon
                    },
                    radius = 5000.0
                }
            },
            excludedTypes = tiposExcluidos
        };

        var jsonContent = JsonConvert.SerializeObject(requestBody);
        var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        using (var client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("X-Goog-Api-Key", ApiKey);
            client.DefaultRequestHeaders.Add("X-Goog-FieldMask", "places.displayName,places.primaryTypeDisplayName,places.id");

            var response = await client.PostAsync(url, httpContent);
            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<dynamic>(jsonResponse);

                // Procesar los resultados y mostrar los nombres en un MessageBox
                var results = data.places;
                string places = "";
                string types = "";
                int index = 1;
                foreach (var result in results)
                {
                    string name = result["displayName"]["text"].ToString();
                    string id = result["id"].ToString();
                    // Check if primaryTypeDisplayName is available
                    if (result["primaryTypeDisplayName"] != null)
                    {
                        types = result["primaryTypeDisplayName"]["text"].ToString();
                        places += $"{index}. {name} - {id} - Tipo: {types}\n";

                        lugaresInteres.Add(name, types);
                    }
                    else
                    {
                        places += $"{index}. {name}  - {id} - Tipo: Otro\n";
                        lugaresInteres.Add(name, "Otro");
                    }

                    index++;
                }

                await DisplayAlert("Top 5 Lugares de Interés Cercanos", places, "OK");

                await GetPlacePhoto(placeId, photosName);
            }
            else
            {
                await DisplayAlert("Error", "Error en la solicitud: " + response.ReasonPhrase, "OK");
            }
        }
    }

    private async Task GetPlacePhoto(string placeId, string photosName)
    {
        string url = $"https://places.googleapis.com/v1/{photosName}/media?maxHeightPx=2000&maxWidthPx=2000&key={ApiKey}";

        if (photosName == "Sin Fotos")
        {
            imgLocalidad = "https://clfynwobrskueprtvnmg.supabase.co/storage/v1/object/public/ComunidadAutonoma/placeholder.jpg";
        }
        else
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    byte[] imageBytes = await response.Content.ReadAsByteArrayAsync();


                    await subirImagenSupabase(imageBytes);

                    await insertarLocalidad();

                }
            }
        }
    }

    private async Task subirImagenSupabase(byte[] fileBytes) // Cambiado de void a Task
    {
        Supabase.Client cliente = new Supabase.Client(ConexionSupabase.SUPABASE_URL, ConexionSupabase.SUPABASE_KEY);

        if (fileBytes != null)
        {
            string nombreArchivo = nombreLocalidad.Transformacion();

            var fileName = $"{nombreArchivo}_{DateTime.Now.Ticks}.png";
            var response = await cliente.Storage.From("imagenesLocalidad").Upload(fileBytes, fileName);

            if (response != null)
            {
                await DisplayAlert("Detalles", "Imagen subida exitosamente", "OK");
                imgLocalidad = $"{ConexionSupabase.SUPABASE_URL}/storage/v1/object/public/imagenesLocalidad/{fileName}";
            }
            else
            {
                await DisplayAlert("Error", "Error al subir la imagen", "OK");
            }
        }
        else
        {
            await DisplayAlert("Error", "No hay imagen para subir", "OK");
        }
    }

    private async Task insertarLocalidad() // Cambiado de void a Task
    {
        Provincia p = await ObtenerProvinciaPorNombre();

        idProvincia = p.IdProvincia;

        Localidad l = new Localidad()
        {
            NombreLocalidad = nombreLocalidad,
            Coordenada1 = latitud,
            Coordenada2 = longitud,
            ImagenLocalidad = imgLocalidad,
            IdProvincia = idProvincia
        };

        Supabase.Client cliente = new Supabase.Client(ConexionSupabase.SUPABASE_URL, ConexionSupabase.SUPABASE_KEY);

        var response = await cliente.From<Localidad>().Insert(l);




        if (response.Models.Count > 0)
        {
            Localidad localidadInsertada = response.Models[0];
            long idLocalidadInsertada = localidadInsertada.IdLocalidad;

            LugarInteres lu;
            foreach (var item in lugaresInteres)
            {
                lu = new LugarInteres();

                lu.Lugar = item.Key;
                lu.Tipo = item.Value;
                lu.IdLocalidad = idLocalidadInsertada;

                await cliente.From<LugarInteres>().Insert(lu);
            }

            await DisplayAlert("Detalles", "Todo Insertado", "OK");
            lugaresInteres.Clear();
        }




    }



    private async Task<Provincia> ObtenerProvinciaPorNombre()
    {
        Supabase.Client cliente = new Supabase.Client(ConexionSupabase.SUPABASE_URL, ConexionSupabase.SUPABASE_KEY);

        // Obtener todas las provincias
        var response = await cliente.From<Provincia>().Get();
        var provincias = response.Models;

        // Filtrar la provincia por nombre
        var provincia = provincias.FirstOrDefault(p => p.NombreProvincia == nombreProvincia);

        return provincia;
    }

    private class Place
    {
        public string Description { get; set; }
        public string PlaceId { get; set; }

        public Place(string description, string placeId)
        {
            Description = description;
            PlaceId = placeId;
        }
    }
}