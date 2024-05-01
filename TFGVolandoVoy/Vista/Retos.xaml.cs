using Supabase;
using Supabase.Interfaces;
using TFGVolandoVoy.Modelo;

namespace TFGVolandoVoy.Vista;

public partial class Retos : ContentPage
{
    private readonly Supabase.Client _supabaseClient;
    private long idLocalidad;
    private Client client;

    public string TextoReto { get; set; }

    public Retos(long idLocalidad, Supabase.Client supabaseClient)
    {
        _supabaseClient = supabaseClient;
        this.idLocalidad = idLocalidad;
        TextoReto = ConsultarTexto(idLocalidad);
        InitializeComponent();
    }

    private string? ConsultarTexto(long idLocalidad)
    {
        throw new NotImplementedException();
    }

    public Retos() : this(new Supabase.Client(ConexionSupabase.SUPABASE_URL, ConexionSupabase.SUPABASE_KEY))
    {
    }

    
}