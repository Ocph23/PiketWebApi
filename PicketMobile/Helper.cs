using System.Text.Json.Serialization;
using System.Text.Json;
using System.Text;

namespace PicketMobile
{
    internal class Helper
    {
        public static JsonSerializerOptions JsonOption { get; set; } = new() { PropertyNameCaseInsensitive = true, ReferenceHandler = ReferenceHandler.Preserve };

        internal static string GetIndonesiaDayName(string v)
        {
            return v switch
            {
                "Monday" => "Senin",
                "Tuesday" => "Selasa",
                "Wednesday" => "Rabu",
                "Thursday" => "Kamis",
                "Friday" => "Jumat",
                "Saturday" => "Sabtu",
                _ => ""

            };
        }


        public static string? GetInitial(string? name)
        {
            if (string.IsNullOrEmpty(name))
                return string.Empty;

            StringBuilder sb = new StringBuilder();
            foreach (var item in name.Split(" "))
            {
                sb.Append(item.Substring(0, 1).ToUpper());
            }
            return sb.ToString();
        }
    }
}
