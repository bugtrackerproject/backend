using System.Text.Json;
using System.Text.RegularExpressions;
using System.Text.Json.Serialization;

namespace bugtracker_backend_net.Data
{
    public class EnumWithSpacesConverter<T> : JsonConverter<T> where T : Enum
    {
        // Converts enum value to string with spaces added between camel case words
        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            string enumString = value.ToString();
            string formattedString = AddSpacesToCamelCase(enumString);
            writer.WriteStringValue(formattedString);
        }

        // Converts string with spaces back to enum
        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string enumString = reader.GetString();
            return (T)Enum.Parse(typeToConvert, RemoveSpacesFromString(enumString));
        }

        // Adds spaces between camel case words
        private string AddSpacesToCamelCase(string input)
        {
            return Regex.Replace(input, @"([a-z])([A-Z])", "$1 $2");
        }

        // Removes spaces from a string (for deserialization)
        private string RemoveSpacesFromString(string input)
        {
            return input.Replace(" ", string.Empty);
        }
    }
}
