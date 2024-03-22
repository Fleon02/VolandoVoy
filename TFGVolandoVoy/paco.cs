using Postgrest.Attributes;
using Postgrest.Models;
using Supabase;

// Dado el siguiente modelo (Paco.cs)
[Table("paco")]
public class Paco : BaseModel
{
    [PrimaryKey("id_paco")]
    public int IdPaco { get; set; }

    [Column("nasio_en")]
    public string? NasioEn { get; set; }

    // Otras propiedades si es necesario
}
