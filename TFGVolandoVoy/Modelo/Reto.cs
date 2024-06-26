﻿using Postgrest.Attributes;
using Postgrest.Models;

[Table("retos")]
public class Reto : BaseModel
{
    [PrimaryKey("idreto")]
    public long IdReto { get; set; }

    [Column("idlocalidad")]
    public long IdLocalidad { get; set; }

    [Column("reto")]
    public string? DescripcionReto { get; set; }

    [Column("superado")]
    public bool Superado { get; set; }

    [Column("imagenRetoCompletado")]
    public string? ImagenCompletado { get; set; }    

    [Column("imagenRetoPreview")]
    public string? ImagenRetoPreview { get; set; }

    [Column("resumenReto")]
    public string? ResumenDeReto { get; set; }
}
