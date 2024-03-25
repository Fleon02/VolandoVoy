using Postgrest.Attributes;
using Postgrest.Models;

[Table("localidad")]
public class Localidad : BaseModel
{
    [PrimaryKey("idlocalidad")]
    public long IdLocalidad { get; set; }

    [Column("localidad")]
    public string? NombreLocalidad { get; set; }

    [Column("imglocalidad")]
    public string? ImagenLocalidad { get; set; }

    [Column("idprovincia")]
    public long IdProvincia { get; set; }
}
