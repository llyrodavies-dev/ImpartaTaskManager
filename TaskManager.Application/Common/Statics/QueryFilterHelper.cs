using FluentValidation;
using FluentValidation.Results;
using Utility.Filtering;
using Utility.Filtering.Models;

namespace TaskManager.Application.Common.Statics
{
    public static class QueryFilterHelper
    {
        public static void ValidateFilters<T>(FilterModelConfiguration<T> configuration, List<FilterCondition> filters)
        {
            List<ValidationResult> validationResults = FilterValidation.ValidateFilterOptions(ref filters, configuration.ColumnMappingDict);
            if (validationResults!.Exists(x => !x.IsValid))
            {
                var failures = validationResults
                    .Where(x => !x.IsValid)
                    .SelectMany(x => x.Errors)
                    .ToList();

                throw new ValidationException(failures);
            }
        }

        public static string GenerateSortExpression<T>(FilterModelConfiguration<T> configuration, string? sortColumn, bool isDescending, string defaultColumn)
        {
            if (string.IsNullOrWhiteSpace(sortColumn) || !configuration.ColumnMappingDict.ContainsKey(sortColumn))
            {
                sortColumn = defaultColumn;
            }

            return isDescending ? $"{sortColumn} DESC" : sortColumn;
        }

        public static (string Query, Dictionary<string, object> Args) ParseFilters(List<FilterCondition> filters)
        {
            return FilterParser.ConvertToDynamicLinqQuery(filters);
        }
    }
}
