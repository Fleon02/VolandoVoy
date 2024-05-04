using Postgrest.Attributes;
using Postgrest.Models;

[Table("retos")]
public class Retoss : BaseModel
{
    [PrimaryKey("idreto")]
    public long IdReto { get; set; }

    [Column("idlocalidad")]
    public long IdLocalidad { get; set; }

    [Column("reto")]
    public string? DescripcionReto { get; set; }

    [Column("superado")]
    public bool Superado { get; set; }
}
