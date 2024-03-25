using Postgrest.Attributes;
using Postgrest.Models;

[Table("provincia")]
public class Provincia : BaseModel
{
    [PrimaryKey("idprovincia")]
    public long IdProvincia { get; set; }

    [Column("provincia")]
    public string? NombreProvincia { get; set; }

    [Column("imgprovincia")]
    public string? ImagenProvincia { get; set; }

    [Column("com_autonoma")]
    public string? ComunidadAutonoma { get; set; }
}
