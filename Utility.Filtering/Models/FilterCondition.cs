using System.Text.Json.Serialization;

namespace Utility.Filtering.Models
{
    public class FilterCondition
    {
        public string FieldName { get; set; } = string.Empty;
        public FilterOperator Operator { get; set; }
        public List<string> Values { get; set; } = [];

        [JsonIgnore]
        // This is an internal property and not needed by the UI.
        public Type Type { get; set; } = typeof(string);
    }
}
