using DocumentFormat.OpenXml.Bibliography;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Supabase.Interfaces;
using System.Net;
using System.Text;
using TFGVolandoVoy.Modelo;
using TraductorTipos;
using static System.Net.WebRequestMethods;
using Map = Microsoft.Maui.Controls.Maps.Map;

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

    private String PlaceID = "";

    private String PhotoName = "";

    List<LugarInteres> lugaresIntereses;

    public CrearLocalidad()
    {
        InitializeComponent();

        Shell.SetFlyoutBehavior(this, FlyoutBehavior.Disabled);

        suggestions = new List<Place>(); // Initialize empty suggestion list

        lugaresIntereses = new List<LugarInteres>();


    }

    private async void textBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
    {
        string input = e.NewTextValue;
        if (input.Length >= 3)
        {
            suggestions = await ObtenerSitios(input);
            listBoxPlaces.ItemsSource = null;
            listBoxPlaces.ItemsSource = suggestions;
        }
        else
        {
            listBoxPlaces.ItemsSource = null;
        }
    }

    private async Task<List<Place>> ObtenerSitios(string input)
    {
        using (var client = new HttpClient())
        {

            List<Place> localityResults = await GetAutocompleteSuggestionsAsync(input, "locality");

            List<Place> areaLevel3Results = await GetAutocompleteSuggestionsAsync(input,"administrative_area_level_3");

            // Combine Results
            List<Place> allResults = new List<Place>();
            allResults.AddRange(localityResults);
            allResults.AddRange(areaLevel3Results);

            return allResults;
        }
    }

    private async Task<List<Place>> GetAutocompleteSuggestionsAsync(string input, string tipo)
    {
        using (var client = new HttpClient())
        {
            var requestBody = new
            {
                input = input,
                languageCode = "es",
                includedRegionCodes = "es",
                includedPrimaryTypes = tipo,
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
            PlaceID = selectedPlace.PlaceId;

            await DisplayAlert("Place Selected", $"{PlaceID} - {selectedPlace.Description}", "OK");



            GetAddressDetails();
        }
    }

    private async void GetAddressDetails()
    {
        string url = $"https://places.googleapis.com/v1/places/{PlaceID}?fields=addressComponents,location,photos&key={ApiKey}&languageCode=es";

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

                double latitude = 0.0;
                double longitude = 0.0;

                if (data != null)
                {
                    var addressComponents = data.addressComponents;
                    if (data.photos != null && data.photos.Count > 0)
                    {
                        PhotoName = data.photos[0]["name"]?.ToString() ?? "Sin Fotos";
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
                                    case "administrative_area_level_3":
                                        nombreLocalidad = longText;
                                        locality = longText;
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

                string addressDetails = $"Localidad: {locality}\nProvincia: {administrativeAreaLevel2}\nComunidad Autonoma: {administrativeAreaLevel1}\nPa�s: {country}\nC�digo Postal: {postalCode}\n\nCoordenadas:\nLatitud: {latitude:F6}\nLongitud: {longitude:F6}";

                //await DisplayAlert("Detalles de la direcci�n", addressDetails, "OK");


                if (SLMapa.Children.Count > 0)
                {
                    SLMapa.Children.RemoveAt(0);
                    lugaresIntereses.Clear();
                }

                var mapView = new Map
                {
                    HeightRequest = 300,
                    VerticalOptions = LayoutOptions.FillAndExpand,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    IsShowingUser = false,
                    MapType = MapType.Street
                };
                var coordenadas = new Location(latitud, longitud);
                Pin pinmapa = new Pin
                {
                    Label = nombreLocalidad + " (" + nombreProvincia + ")", // Nombre del marcador            
                    Location = coordenadas
                };
                mapView.Pins.Add(pinmapa);

                mapView.MoveToRegion(MapSpan.FromCenterAndRadius(coordenadas, Distance.FromKilometers(1)));
                SLMapa.Children.Add(mapView);

                btnConfirmar.IsVisible = true;

            }
        }
    }

    private async Task ShowNearbyPlacesAsync()
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
                        latitude = latitud,
                        longitude = longitud
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
            client.DefaultRequestHeaders.Add("X-Goog-FieldMask", "places.displayName,places.primaryTypeDisplayName,places.id,places.location");

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
                LugarInteres lu;
                foreach (var result in results)
                {
                    lu = new LugarInteres();
                    string name = result["displayName"]["text"].ToString();
                    string id = result["id"].ToString();
                    double latitud;
                    double longitud;
                    // Check if primaryTypeDisplayName is available
                    if (result["primaryTypeDisplayName"] != null)
                    {

                        types = result["primaryTypeDisplayName"]["text"].ToString();
                        places += $"{index}. {name} - {id} - Tipo: {types}\n";

                        latitud = result["location"]["latitude"];
                        longitud = result["location"]["longitude"];

                        lu.Lugar = name;
                        lu.Tipo = TraductorTipos.TraductorTipos.GetTraduccion(types);
                        lu.Latitud = latitud;
                        lu.Longitud = longitud;

                        lugaresIntereses.Add(lu);

                    }
                    else
                    {
                        latitud = result["location"]["latitude"];
                        longitud = result["location"]["longitude"];

                        lu.Lugar = name;
                        lu.Latitud = latitud;
                        lu.Longitud = longitud;
                        lu.Tipo = "Otro";
                        places += $"{index}. {name}  - {id} - Tipo: Otro\n";

                        lugaresIntereses.Add(lu);
                    }

                    index++;
                }

                //await DisplayAlert("Top 5 Lugares de Inter�s Cercanos", places, "OK");

                await GetPlacePhoto();


            }
            else
            {
                await DisplayAlert("Error", "Error en la solicitud: " + response.ReasonPhrase, "OK");
            }
        }
    }



    private async Task GetPlacePhoto()
    {
        string url = $"https://places.googleapis.com/v1/{PhotoName}/media?maxHeightPx=2000&maxWidthPx=2000&key={ApiKey}";

        if (PhotoName == "Sin Fotos" || PhotoName == "")
        {
            imgLocalidad = "https://clfynwobrskueprtvnmg.supabase.co/storage/v1/object/public/ComunidadAutonoma/placeholder.jpg";
            await insertarLocalidad();
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

        Supabase.Client cliente = new Supabase.Client(ConexionSupabase.SUPABASE_URL, ConexionSupabase.SUPABASE_KEY);


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



        var response = await cliente.From<Localidad>().Insert(l);




        if (response.Models.Count > 0)
        {
            Localidad localidadInsertada = response.Models[0];
            long idLocalidadInsertada = localidadInsertada.IdLocalidad;

            foreach (var item in lugaresIntereses)
            {
                item.IdLocalidad = idLocalidadInsertada;

                await cliente.From<LugarInteres>().Insert(item);
            }

            await DisplayAlert("Detalles", "Todo Insertado", "OK");
            lugaresIntereses.Clear();

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


    private async void btnConfirmar_Clicked_1(object sender, EventArgs e)
    {
        Supabase.Client cliente = new Supabase.Client(ConexionSupabase.SUPABASE_URL, ConexionSupabase.SUPABASE_KEY);

        var LocalidadesBBDD = await cliente.From<Localidad>().Get();
        var localidadExistente = LocalidadesBBDD.Models.FirstOrDefault(l =>
                AreCoordinatesEqual(l.Coordenada1, latitud) &&
                AreCoordinatesEqual(l.Coordenada2, longitud));

        if (localidadExistente != null)
        {
            await DisplayAlert("Error", "Lo sentimos, La localidad ya existe.", "OK");
            return;
        }

        await ShowNearbyPlacesAsync();
    }

    private bool AreCoordinatesEqual(double coord1, double coord2, double tolerance = 0.00001)
    {
        return Math.Abs(coord1 - coord2) < tolerance;
    }
}