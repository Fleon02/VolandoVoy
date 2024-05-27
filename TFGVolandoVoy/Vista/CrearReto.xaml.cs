using Supabase.Interfaces;
using System;
using Microsoft.Maui.Controls;
using TFGVolandoVoy.Modelo;
using Supabase.Realtime;
using System.Collections.ObjectModel;

namespace TFGVolandoVoy.Vista;

public partial class CrearReto : ContentPage
{

    public ObservableCollection<Localidad> Localidades { get; set; }
    private readonly Supabase.Client _supabaseClient;
    public CrearReto(Supabase.Client supabaseClient)
	{
		InitializeComponent();
        _supabaseClient = supabaseClient;
        //selector_ciudades.SelectedIndexChanged += selector_ciudades_SelectedIndexChanged;
    }

    //private async Task CargarLocalidades()
    //{
    //    var response = await _supabaseClient.From<Localidad>().Get();
    //    var localidades = response.Models;

    //    if (localidades != null && localidades.Count > 0)
    //    {
    //        Localidades.Clear();
    //        foreach (var localidad in localidades)
    //        {
    //            Localidades.Add(localidad);
    //        }
    //    }
    //    else
    //    {
    //        Console.WriteLine("No se encontraron localidades.");
    //    }

    //    selector_ciudades.ItemsSource = localidades;
    //}
    

    //protected override async void OnAppearing()
    //{
    //    base.OnAppearing();
    //    await CargarLocalidades();
    //}

    //public async void InsertarReto_Clicked(object sender, EventArgs e)
    //{
    //    Localidad localidadSeleccionada = (Localidad)selector_ciudades.SelectedItem;
    //    long idLocalidad = localidadSeleccionada.IdLocalidad;
    //    string descripcionReto = RetoDefinicion.Text;
    //    string tipoReto = TipoReto.Text;


    //    var nuevoReto = new Reto
    //    {
    //        IdLocalidad = idLocalidad,
    //        DescripcionReto = descripcionReto,
    //        Superado = false,
    //        TipoDeReto = tipoReto
    //    };

    //    var insertTask = _supabaseClient.From<Reto>().Insert(nuevoReto);

    //    // Esperar a que se complete la operación de inserción
    //    await insertTask;

    //    // Verificar si la inserción fue exitosa
    //    if (insertTask.IsCompletedSuccessfully)
    //    {
    //        // Inserción exitosa
    //        await DisplayAlert("Éxito", "El reto se ha insertado correctamente.", "OK");

    //        // Limpiar los campos después de la inserción exitosa
    //        RetoDefinicion.Text = "";
    //        TipoReto.Text = "";
    //    }
    //    else
    //    {
    //        // Error al insertar el reto
    //        await DisplayAlert("Error", "No se pudo insertar el reto. Por favor, inténtelo de nuevo.", "OK");
    //    }
    //}

    //private void selector_ciudades_SelectedIndexChanged(object sender, EventArgs e)
    //{
    //    if (selector_ciudades.SelectedIndex != -1)
    //    {
    //        var localidadSeleccionada = (Localidad)selector_ciudades.SelectedItem;
    //        long idLocalidad = localidadSeleccionada.IdLocalidad;
    //        string nombreLocalidad = localidadSeleccionada.NombreLocalidad;

    //    }
    //}


    //private void Selector_ciudades_SelectedIndexChanged(object sender, EventArgs e)
    //{
    //    if (selector_ciudades.SelectedIndex != -1)
    //    {
    //        var localidadSeleccionada = (Localidad)selector_ciudades.SelectedItem;
    //        long idLocalidad = localidadSeleccionada.IdLocalidad;
    //        string nombreLocalidad = localidadSeleccionada.NombreLocalidad;

    //    }
    //}
}