using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TFGVolandoVoy.Modelo
{
    public static class TransformadorPalabraNombreImagen
    {
        public static string QuitarTildes(this string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return text;

            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder
                .ToString()
                .Normalize(NormalizationForm.FormC);
        }

        public static string RemplazarCaracteresEspeciales(this string text)
        {
            Dictionary<char, char> characterMap = new Dictionary<char, char>
            {
                { 'á', 'a' }, { 'é', 'e' }, { 'í', 'i' }, { 'ó', 'o' }, { 'ú', 'u' },
                { 'Á', 'A' }, { 'É', 'E' }, { 'Í', 'I' }, { 'Ó', 'O' }, { 'Ú', 'U' },
                { 'ñ', 'n' }, { 'Ñ', 'N' }
            };

            var stringBuilder = new StringBuilder();

            foreach (var c in text)
            {
                if (characterMap.ContainsKey(c))
                {
                    stringBuilder.Append(characterMap[c]);
                }
                else
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString();
        }

        public static string Transformacion(this string text)
        {
            // Replace spaces with underscores
            text = text.Replace(" ", "_");

            // Remove diacritics and replace special characters
            text = text.QuitarTildes().RemplazarCaracteresEspeciales();

            return text;
        }
    }
}
