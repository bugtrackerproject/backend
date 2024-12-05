using System.Text.Json;

namespace bugtracker_backend_net.Data
{
    public class EnumNamingPolicy : JsonNamingPolicy
    {
        public override string ConvertName(string name)
        {
            return string.Concat(name.Select((x, i) =>
                i > 0 && Char.IsUpper(x) ? $" {x}" : $"{x}"));
        }
    }
}
