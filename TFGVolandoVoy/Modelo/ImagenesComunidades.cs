using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TFGVolandoVoy.Modelo
{
    public class ImagenesComunidades
    {
        public static Dictionary<string, string> ImagenesBanderas { get; set; } = new Dictionary<string, string>()
        {
            { "Andalucía", "https://clfynwobrskueprtvnmg.supabase.co/storage/v1/object/public/ComunidadAutonoma/Andalucia.png" },
            { "Aragón", "https://clfynwobrskueprtvnmg.supabase.co/storage/v1/object/public/ComunidadAutonoma/Aragon.png" },
            { "Asturias", "https://clfynwobrskueprtvnmg.supabase.co/storage/v1/object/public/ComunidadAutonoma/Asturias.png" },
            { "Islas Baleares", "https://clfynwobrskueprtvnmg.supabase.co/storage/v1/object/public/ComunidadAutonoma/Baleares.png" },
            { "Canarias", "https://clfynwobrskueprtvnmg.supabase.co/storage/v1/object/public/ComunidadAutonoma/Canarias.png" },
            { "Cantabria", "https://clfynwobrskueprtvnmg.supabase.co/storage/v1/object/public/ComunidadAutonoma/Cantabria.png" },
            { "Castilla-La Mancha", "https://clfynwobrskueprtvnmg.supabase.co/storage/v1/object/public/ComunidadAutonoma/CastillaLaMancha.png" },
            { "Castilla y León", "https://clfynwobrskueprtvnmg.supabase.co/storage/v1/object/public/ComunidadAutonoma/CastillaLeon.png" },
            { "Cataluña", "https://clfynwobrskueprtvnmg.supabase.co/storage/v1/object/public/ComunidadAutonoma/Cataluna.png" },
            { "Ceuta", "https://clfynwobrskueprtvnmg.supabase.co/storage/v1/object/public/ComunidadAutonoma/Ceuta.png" },
            { "Comunidad Valenciana", "https://clfynwobrskueprtvnmg.supabase.co/storage/v1/object/public/ComunidadAutonoma/ComunidadValenciana.png" },
            { "Extremadura", "https://clfynwobrskueprtvnmg.supabase.co/storage/v1/object/public/ComunidadAutonoma/Extremadura.png" },
            { "Galicia", "https://clfynwobrskueprtvnmg.supabase.co/storage/v1/object/public/ComunidadAutonoma/Galicia.png" },
            { "La Rioja", "https://clfynwobrskueprtvnmg.supabase.co/storage/v1/object/public/ComunidadAutonoma/LaRioja.png" },
            { "Comunidad de Madrid", "https://clfynwobrskueprtvnmg.supabase.co/storage/v1/object/public/ComunidadAutonoma/Madrid.png" },
            { "Melilla", "https://clfynwobrskueprtvnmg.supabase.co/storage/v1/object/public/ComunidadAutonoma/Melilla.png" },
            { "Región de Murcia", "https://clfynwobrskueprtvnmg.supabase.co/storage/v1/object/public/ComunidadAutonoma/Murcia.png" },
            { "Navarra", "https://clfynwobrskueprtvnmg.supabase.co/storage/v1/object/public/ComunidadAutonoma/Navarra.png" },
            { "País Vasco", "https://clfynwobrskueprtvnmg.supabase.co/storage/v1/object/public/ComunidadAutonoma/PaisVasco.png" }
        };

        public static string GetImagen(string comunidadAutonoma)
        {
            if (ImagenesBanderas.ContainsKey(comunidadAutonoma))
            {
                return ImagenesBanderas[comunidadAutonoma];
            }
            else
            {
                return "https://clfynwobrskueprtvnmg.supabase.co/storage/v1/object/public/ComunidadAutonoma/placeholder.jpg";
            }
        }
    }
}
