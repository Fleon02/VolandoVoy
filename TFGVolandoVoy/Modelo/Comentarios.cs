using Postgrest.Attributes;
using Postgrest.Models;

[Table("comentarios")]
public class Comentarios : BaseModel
{
    [PrimaryKey("idcomentario")]
    public long IdComentario { get; set; }
    [Column("idusuario")]
    public long IdUsuario { get; set; }

    [Column("idlocalidad")]
    public long IdLocalidad { get; set; }

    [Column("comentario")]
    public string? Comentario { get; set; }

    [Column("valoracion")]
    public int Valoracion { get; set; }
}
