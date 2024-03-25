using Postgrest.Attributes;
using Postgrest.Models;

[Table("comentarios")]
public class Comentarios : BaseModel
{
    [PrimaryKey("idusuario")]
    public long IdUsuario { get; set; }

    [Column("idlocalidad")]
    public long IdLocalidad { get; set; }

    [Column("comentario")]
    public string? Comentario { get; set; }

    [Column("valoracion")]
    public short Valoracion { get; set; }
}
