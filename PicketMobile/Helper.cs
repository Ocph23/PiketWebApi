using System.Text.Json.Serialization;
using System.Text.Json;

namespace PicketMobile
{
    internal class Helper
    {
        public static JsonSerializerOptions JsonOption { get; set; } = new() { PropertyNameCaseInsensitive = true, ReferenceHandler = ReferenceHandler.Preserve };

    }
}
