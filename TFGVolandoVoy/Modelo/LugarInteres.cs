using Postgrest.Attributes;
using Postgrest.Models;

[Table("lugarinteres")]
public class LugarInteres : BaseModel
{
    [PrimaryKey("idlugar")]
    public long IdLugar { get; set; }

    [Column("idlocalidad")]
    public long IdLocalidad { get; set; }

    [Column("lugar")]
    public string? Lugar { get; set; }

    [Column("tipo")]
    public string? Tipo { get; set; }

    [Column("latitud")]
    public double Latitud { get; set; }

    [Column("longitud")]
    public double Longitud { get; set; }
}
