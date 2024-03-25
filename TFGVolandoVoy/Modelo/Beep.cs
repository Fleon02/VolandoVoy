using Postgrest.Attributes;
using Postgrest.Models;

[Table("beep")]
public class Beep : BaseModel
{
    [PrimaryKey("idusuario")]
    public long IdUsuario { get; set; }

    [Column("hash_contrasena")]
    public string? HashContrasena { get; set; }

    [Column("salt")]
    public string? Salt { get; set; }
}
