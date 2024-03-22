using Microsoft.Maui.Controls;
using Supabase;
using Postgrest.Attributes;
using Postgrest.Models;
using System;
using System.Threading.Tasks;

namespace TFGVolandoVoy
{
    public partial class MainPage : ContentPage
    {
        private int count = 0;
        private Supabase.Client _supabase;

        public MainPage()
        {
            InitializeComponent();

            // Configurar Supabase
            var url = "https://clfynwobrskueprtvnmg.supabase.co";
            var key = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6ImNsZnlud29icnNrdWVwcnR2bm1nIiwicm9sZSI6ImFub24iLCJpYXQiOjE3MTExMjI4OTYsImV4cCI6MjAyNjY5ODg5Nn0.yil-jPNh7m6uk9veYRnnAB2Cjt51lTyCbu18oiluk98";

            var options = new SupabaseOptions
            {
                AutoRefreshToken = true,
                AutoConnectRealtime = true,
            };
            _supabase = new Supabase.Client(url, key, options);
        }

        private async void OnCounterClicked(object sender, EventArgs e)
        {

            SemanticScreenReader.Announce(CounterBtn.Text);

            // Obtener los datos de la tabla Paco y mostrarlos en el texto del botón
            try
            {
                var result = await _supabase.From<Paco>().Get();

                if (result != null && result.Models.Count > 0)
                {
                    var paco = result.Models[0];
                    CounterBtn.Text = $"ID: {paco.IdPaco}, Nasio En: {paco.NasioEn}";
                }
                else
                {
                    CounterBtn.Text = "No hay datos disponibles";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener los datos de la tabla Paco: {ex.Message}");
                CounterBtn.Text = "Error al obtener datos";
            }
        }
    }
}
