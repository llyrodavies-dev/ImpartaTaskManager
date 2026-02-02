using System.ComponentModel;
using Utility.Filtering.Exceptions;
using Utility.Filtering.Models;

namespace Utility.Filtering
{
    public static class FilterParser
    {
        /// <summary>
        /// Converts a list of filter conditions to a dynamic LINQ query string and parameters.
        /// </summary>
        /// <param name="filters"> Lisf of FilterConsitions to be converted. </param>
        /// <returns> Dynamic LINQ query string and parameters </returns>
        /// <exception cref="ArgumentException"></exception>
        public static (string QueryString, Dictionary<string, object> Arguments) ConvertToDynamicLinqQuery(List<FilterCondition> filters)
        {
            if (filters == null || filters.Count == 0)
                return (string.Empty, new Dictionary<string, object>());

            List<string> queryStrings = [];
            Dictionary<string, object> args = new();
            // Track parameter positions
            int paramIndex = 0;

            foreach (FilterCondition condition in filters)
            {
                queryStrings.Add(ConvertToDynamicLinqQuery(condition.FieldName, condition.Operator, condition.Values, condition.Type, ref args, ref paramIndex));
            }

            return (string.Join(" AND ", queryStrings), args);
        }

        private static string ConvertToDynamicLinqQuery(string fieldName, FilterOperator op, List<string> values, Type type, ref Dictionary<string, object> args, ref int paramIndex)
        {
            var parameterPlaceholders = new List<string>();
            string query;

            try
            {
                switch (op)
                {
                    case FilterOperator.Equals:
                        query = $"{fieldName} == @{paramIndex}";
                        AddValueAndIncrementCount(values[0], type, ref paramIndex, ref args);
                        break;

                    case FilterOperator.NotEquals:
                        query = $"{fieldName} != @{paramIndex}";
                        AddValueAndIncrementCount(values[0], type, ref paramIndex, ref args);
                        break;

                    case FilterOperator.LessThan:
                        query = $"{fieldName} < @{paramIndex}";
                        AddValueAndIncrementCount(values[0], type, ref paramIndex, ref args);
                        break;

                    case FilterOperator.LessThanOrEqual:
                        query = $"{fieldName} <= @{paramIndex}";
                        AddValueAndIncrementCount(values[0], type, ref paramIndex, ref args);
                        break;

                    case FilterOperator.GreaterThan:
                        query = $"{fieldName} > @{paramIndex}";
                        AddValueAndIncrementCount(values[0], type, ref paramIndex, ref args);
                        break;

                    case FilterOperator.GreaterThanOrEqual:
                        query = $"{fieldName} >= @{paramIndex}";
                        AddValueAndIncrementCount(values[0], type, ref paramIndex, ref args);
                        break;

                    case FilterOperator.In:
                        for (int i = 0; i < values.Count; i++)
                        {
                            parameterPlaceholders.Add($"@{paramIndex}");
                            AddValueAndIncrementCount(values[i], type, ref paramIndex, ref args);
                        }
                        query = ($"{fieldName} IN ({string.Join(", ", parameterPlaceholders)})");
                        break;

                    case FilterOperator.NotIn:
                        for (int i = 0; i < values.Count; i++)
                        {
                            parameterPlaceholders.Add($"@{paramIndex}");
                            AddValueAndIncrementCount(values[i], type, ref paramIndex, ref args);
                        }
                        query = ($"NOT ({fieldName} IN ({string.Join(", ", parameterPlaceholders)}))");
                        break;

                    case FilterOperator.Between:
                        if (values.Count != 2)
                            throw new FilterException("Between operator requires exactly two values.");

                        query = $"{fieldName} >= @{paramIndex} AND {fieldName} <= @{paramIndex + 1}";
                        AddValueAndIncrementCount(values[0], type, ref paramIndex, ref args);
                        AddValueAndIncrementCount(values[1], type, ref paramIndex, ref args);
                        break;

                    case FilterOperator.IsNull:
                        query = $"{fieldName} = null";
                        break;

                    case FilterOperator.Contains:
                        query = $"{fieldName}.Contains(@{paramIndex})";
                        AddValueAndIncrementCount(values[0], type, ref paramIndex, ref args);
                        break;

                    default:
                        query = string.Empty;
                        break;
                }
            }
            catch (FilterException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new FilterException($"Error parsing filter condition: {ex.Message}");
            }

            return query;
        }

        private static void AddValueAndIncrementCount(string value, Type type, ref int count, ref Dictionary<string, object> args)
        {
            var converter = TypeDescriptor.GetConverter(type);
            var result = converter.ConvertFrom(value);

            args.Add($"@{count}", result ?? "null");
            count += 1;
        }
    }
}
