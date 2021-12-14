using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace KafkaTesting.ksqlDB.Extensions
{
    internal static class KsqlExtensions
    {
        public static bool TryParse(this string input, out JArray array)
        {
            try
            {
                var output = JArray.Parse(input);
                array = output;
                return true;
            }
            catch (JsonReaderException)
            {
                array = new JArray();
                return false;
            }
        }
    }
}
