using System.Reflection;

namespace Utility.Filtering
{
    public class FilterModelConfiguration<T>
    {
        public Dictionary<string, Type> ColumnMappingDict { get; }

        public FilterModelConfiguration()
        {
            ColumnMappingDict = GenerateDictionaryItems();
        }

        private static Dictionary<string, Type> GenerateDictionaryItems()
        {
            Dictionary<string, Type> keyValuePairs = [];

            Type t = typeof(T);

            foreach (PropertyInfo prop in t.GetProperties())
            {
                if (prop.PropertyType.IsClass && prop.PropertyType.Assembly.FullName == t.Assembly.FullName)
                {
                    foreach (PropertyInfo subProb in prop.PropertyType.GetProperties())
                    {
                        keyValuePairs.Add($"{prop.Name}.{subProb.Name}", subProb.PropertyType);
                    }
                }
                else
                {
                    keyValuePairs.Add(prop.Name, prop.PropertyType);
                }
            }
            return keyValuePairs;
        }
    }
}
