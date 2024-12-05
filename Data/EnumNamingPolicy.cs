using System.Text.Json;
using System.Text.RegularExpressions;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace bugtracker_backend_net.Data
{
    public class EnumWithSpacesConverter<T> : JsonConverter<T> where T : struct, Enum
    {
        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var enumString = reader.GetString();
            if (string.IsNullOrEmpty(enumString))
            {
                return default(T);
            }

            var enumValue = Enum.TryParse<T>(enumString.Replace(" ", string.Empty), true, out var result)
                ? result
                : default(T);

            return enumValue;
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            string enumString = value.ToString();
            string formattedString = AddSpacesToCamelCase(enumString);
            writer.WriteStringValue(formattedString);
        }

        private string AddSpacesToCamelCase(string input)
        {
            return Regex.Replace(input, @"([a-z])([A-Z])", "$1 $2");
        }
    }
}
