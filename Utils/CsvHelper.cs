using System.Text;

namespace Quiniela.Utils
{
    public static class CsvHelper
    {
        public static byte[] GenerateCsv<T>(IEnumerable<T> data, Dictionary<string, Func<T, object?>> columns)
        {
            var sb = new StringBuilder();

            // Header
            sb.AppendLine(string.Join(",", columns.Keys.Select(EscapeCsv)));

            // Filas
            foreach (var item in data)
            {
                var values = columns.Values.Select(getter =>
                {
                    var value = getter(item);
                    return EscapeCsv(value?.ToString() ?? string.Empty);
                });
                sb.AppendLine(string.Join(",", values));
            }

            // BOM para que Excel detecte correctamente UTF-8 (caracteres con tildes)
            var bom = new byte[] { 0xEF, 0xBB, 0xBF };
            var content = Encoding.UTF8.GetBytes(sb.ToString());
            return bom.Concat(content).ToArray();
        }

        private static string EscapeCsv(string value)
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;

            // Si contiene comas, comillas o saltos de línea hay que escapar
            if (value.Contains(',') || value.Contains('"') || value.Contains('\n') || value.Contains('\r'))
            {
                return $"\"{value.Replace("\"", "\"\"")}\"";
            }

            return value;
        }
    }
}