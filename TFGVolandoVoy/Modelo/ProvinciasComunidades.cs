using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TFGVolandoVoy.Modelo
{
    public class ProvinciasComunidades
    {
        public static Dictionary<string, string> ProvinciaComunidad { get; set; } = new Dictionary<string, string>()
        {
            { "Álava", "País Vasco" },
            { "Albacete", "Castilla-La Mancha" },
            { "Alicante", "Comunidad Valenciana" },
            { "Almería", "Andalucía" },
            { "Asturias", "Principado de Asturias" },
            { "Ávila", "Castilla y León" },
            { "Badajoz", "Extremadura" },
            { "Illes Balears", "Islas Baleares" },
            { "Barcelona", "Cataluña" },
            { "Burgos", "Castilla y León" },
            { "Cáceres", "Extremadura" },
            { "Cádiz", "Andalucía" },
            { "Cantabria", "Cantabria" },
            { "Castellón", "Comunidad Valenciana" },
            { "Ciudad Real", "Castilla-La Mancha" },
            { "Córdoba", "Andalucía" },
            { "Cuenca", "Castilla-La Mancha" },
            { "Girona", "Cataluña" },
            { "Granada", "Andalucía" },
            { "Gipuzkoa", "País Vasco" },
            { "Huelva", "Andalucía" },
            { "Huesca", "Aragón" },
            { "Jaén", "Andalucía" },
            { "La Coruña", "Galicia" },
            { "La Rioja", "La Rioja" },
            { "Las Palmas", "Canarias" },
            { "León", "Castilla y León" },
            { "Lérida", "Cataluña" },
            { "Lugo", "Galicia" },
            { "Madrid", "Comunidad de Madrid" },
            { "Málaga", "Andalucía" },
            { "Melilla", "Melilla" },
            { "Murcia", "Región de Murcia" },
            { "Navarra", "Navarra" },
            { "Ourense", "Galicia" },
            { "Palencia", "Castilla y León" },
            { "Pontevedra", "Galicia" },
            { "Salamanca", "Castilla y León" },
            { "Santa Cruz de Tenerife", "Canarias" },
            { "Segovia", "Castilla y León" },
            { "Vizcaya", "País Vasco" },
            { "Sevilla", "Andalucía" },
            { "Soria", "Castilla y León" },
            { "Tarragona", "Cataluña" },
            { "Teruel", "Aragón" },
            { "Toledo", "Castilla-La Mancha" },
            { "Valencia", "Comunidad Valenciana" },
            { "Valladolid", "Castilla y León" },
            { "Zamora", "Castilla y León" },
            { "Zaragoza", "Aragón" }
        };

        public static string GetComunidadAutonoma(string provincia)
        {
            if (ProvinciaComunidad.ContainsKey(provincia))
            {
                return ProvinciaComunidad[provincia];
            }
            else
            {
                return "Desconocido"; // Puedes cambiar este valor a lo que prefieras para los casos donde no se encuentre la provincia.
            }
        }
    }
}
