using Newtonsoft.Json.Converters;
using System.Text.Json.Serialization;

namespace Dislinkt.Saga.Data
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Seniority
    {
        Junior = 0,
        Medior = 1,
        Senior = 2
    }
}
