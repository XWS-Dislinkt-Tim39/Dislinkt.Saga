using Newtonsoft.Json.Converters;
using System.Text.Json.Serialization;

namespace Dislinkt.Saga.Data
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum GenderData
    {
        /// <summary>
        /// Male
        /// </summary>
        Male = 0,
        /// <summary>
        /// Female
        /// </summary>
        Female = 1
    }
}
