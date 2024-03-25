using Postgrest.Attributes;
using Postgrest.Models;

[Table("usuarios")]
public class Usuario : BaseModel
{
    public enum RolUsuario
    {
        admin,
        usuario
    }

    [PrimaryKey("idusuario")]
    public long IdUsuario { get; set; }

    [Column("nombre_usuario")]
    public string? NombreUsuario { get; set; }

    [Column("apellidos_usuario")]
    public string? ApellidosUsuario { get; set; }

    [Column("email_usuario")]
    public string? EmailUsuario { get; set; }

    [Column("rol")]
    public RolUsuario Rol { get; set; }

    [Column("fecha_alta")]
    public DateTime? FechaAlta { get; set; }

    [Column("imagen_usuario")]
    public string? ImagenUsuario { get; set; }
}
