using System.Globalization;
using System.Text.RegularExpressions;
using Utility.Filtering.Models;
using FluentValidation;
using FluentValidation.Results;

namespace Utility.Filtering
{
    public class FilterValidation
    {
        public static List<ValidationResult> ValidateFilterOptions(ref List<FilterCondition> filterOptions, Dictionary<string, Type> columnNamesDict)
        {
            List<ValidationResult> validationResults = new();
            foreach (FilterCondition filterOption in filterOptions)
            {
                Type? propertyType = columnNamesDict.GetValueOrDefault(filterOption.FieldName);

                if (propertyType == null)
                {
                    filterOption.FieldName = string.Empty;
                    propertyType = typeof(string);
                }

                switch (propertyType.BaseType)
                {
                    case Type stringType when stringType == typeof(string):
                        validationResults.Add(new FilterOptionStringValidator().Validate(filterOption));
                        filterOption.Type = typeof(string);
                        break;
                    case Type boolType when boolType == typeof(bool):
                        validationResults.Add(new FilterOptionBoolValidator().Validate(filterOption));
                        filterOption.Type = typeof(bool);
                        break;
                    case Type dateTimeType when dateTimeType == typeof(DateTime):
                        validationResults.Add(new FilterOptionDateTimeValidator().Validate(filterOption));
                        filterOption.Type = typeof(DateTime);
                        break;
                    case Type doubleType when doubleType == typeof(double):
                        validationResults.Add(new FilterOptionNumberValidator<double>("Double").Validate(filterOption));
                        filterOption.Type = typeof(double);
                        break;
                    case Type decimalType when decimalType == typeof(decimal):
                    case Type decimalTypeNull when decimalTypeNull == typeof(decimal?):
                        validationResults.Add(new FilterOptionNumberValidator<decimal>("Decimal").Validate(filterOption));
                        filterOption.Type = typeof(decimal);
                        break;
                    case Type intType when intType == typeof(int):
                        validationResults.Add(new FilterOptionNumberValidator<int>("Int").Validate(filterOption));
                        filterOption.Type = typeof(int);
                        break;
                    case Type guidType when guidType == typeof(Guid):
                        validationResults.Add(new FilterOptionGuidValidator().Validate(filterOption));
                        filterOption.Type = typeof(Guid);
                        break;
                    case Type enumType when enumType == typeof(Enum):
                        validationResults.Add(new FilterOptionEnumValidator().Validate(filterOption));
                        filterOption.Type = typeof(int);
                        break;
                    default:
                        validationResults.Add(new FilterOptionDefaultValidator().Validate(filterOption));
                        filterOption.Type = typeof(string);
                        break;
                }
            }
            return validationResults;
        }

        #region Fluent Validation

        sealed class FilterOptionDefaultValidator : AbstractValidator<FilterCondition>
        {
            public FilterOptionDefaultValidator()
            {
                RuleFor(x => x.FieldName).NotEmpty().WithMessage("Default Filter Validation failed as {PropertyName} was empty.");
                RuleFor(x => x.Operator).NotEmpty().WithMessage("Default Filter Validation failed as {PropertyName} was empty.");
                RuleFor(x => x.Operator).NotEqual(FilterOperator.None).WithMessage("Default Filter Validation failed as {PropertyName} returned Operator.None.");
                When(x => x.Operator != FilterOperator.IsNull, () =>
                {
                    RuleFor(x => x.Values).NotEmpty().WithMessage("Default Filter Validation failed as {PropertyName} was empty.");
                });
            }
        }

        sealed class FilterOptionStringValidator : AbstractValidator<FilterCondition>
        {
            private readonly int _maxNumberOfCharactersPerValue = 15;
            private readonly int _maxNumberOfValues = 4;

            private static readonly List<FilterOperator> _validOperators = new()
            {
                FilterOperator.Equals,
                FilterOperator.Contains,
                FilterOperator.IsNull,
                FilterOperator.In
            };

            public FilterOptionStringValidator()
            {
                Include(new FilterOptionDefaultValidator());
                RuleFor(x => x.Operator).Must(x => _validOperators.Contains(x!)).WithMessage("String Filter Validation failed as {PropertyName} '{PropertyValue}' is not an accepted {PropertyName}.");
                RuleFor(x => x.Values).Must(y => MultipleValueValidators.ValidateValueLengthsAndCount(y, 15, 4))
                    .When(x => x.Operator == FilterOperator.In).WithMessage("String Filter Validation failed as one or more of the {PropertyName}s '{PropertyValue}' exceeded the expected character limit of " + _maxNumberOfCharactersPerValue + " characters or the number of user vales provided exceeded " + _maxNumberOfValues + ".");
                RuleFor(x => x.Values).Must(z => MultipleValueValidators.ValidateCharactersProvided(z[0]))
                    .When(x => x.Operator == FilterOperator.In).WithMessage("String Filter Validation failed as one or more of the {PropertyName}s '{PropertyValue}' contains invalid characters.");
            }
        }

        sealed class FilterOptionBoolValidator : AbstractValidator<FilterCondition>
        {
            private static readonly List<FilterOperator> _validOperators = new()
            {
                FilterOperator.Equals
            };

            public FilterOptionBoolValidator()
            {
                Include(new FilterOptionDefaultValidator());
                RuleFor(x => x.Operator).Must(x => _validOperators.Contains(x!)).WithMessage("Bool Filter Validation failed as {PropertyName} '{PropertyValue}' is not an accepted {PropertyName}.");
                RuleFor(x => x.Values).Must(x => bool.TryParse(x[0], out bool res)).WithMessage("Bool Filter Validation failed as {PropertyName} '{PropertyValue}' cannot be converted to bool.");
            }
        }

        sealed class FilterOptionNumberValidator<T> : AbstractValidator<FilterCondition>
        {
            private readonly List<FilterOperator> _validOperators = new()
            {
                FilterOperator.Equals,
                FilterOperator.NotEquals,
                FilterOperator.GreaterThan,
                FilterOperator.LessThan,
                FilterOperator.Between,
                FilterOperator.In,
                FilterOperator.NotIn
            };

            public FilterOptionNumberValidator(string propertyType)
            {
                Include(new FilterOptionDefaultValidator());
                RuleFor(x => x.Operator).Must(x => _validOperators.Contains(x!)).WithMessage(propertyType + " Filter Validation failed as {PropertyName} '{PropertyValue}' is not an accepted {PropertyName}.");
                RuleFor(x => x.Values).Must(x => TryParseType<T>(x[0]))
                    .When(x => x.Operator != FilterOperator.Between && x.Operator != FilterOperator.In && x.Operator != FilterOperator.NotIn).WithMessage(propertyType + " Filter Validation failed as {PropertyName} '{PropertyValue}' cannot be converted to " + propertyType + ".");
                RuleFor(x => x.Values).Must(x => x.Count == 2)
                    .When(x => x.Operator == FilterOperator.Between).WithMessage(propertyType + " Filter Validation failed as only one {PropertyName} '{PropertyValue}' was provided for Between Operator.");
                RuleFor(x => x.Values).Must(y => MultipleValueValidators.GenericCheckValidType<T>(y, 2))
                    .When(x => x.Operator == FilterOperator.Between).WithMessage(propertyType + " Filter Validation failed as either one or both of the {PropertyName}s '{PropertyValue}' cannot be converted to " + propertyType + ".");
                RuleFor(x => x.Values).Must(y => MultipleValueValidators.GenericCheckValidType<T>(y))
                    .When(x => x.Operator == FilterOperator.In || x.Operator == FilterOperator.NotIn).WithMessage(propertyType + " Filter Validation failed as one or more of the {PropertyName}s '{PropertyValue}' cannot be converted to " + propertyType + ".");
            }
        }

        sealed class FilterOptionDateTimeValidator : AbstractValidator<FilterCondition>
        {
            private static readonly List<FilterOperator> _validOperators = new()
            {
                FilterOperator.Equals,
                FilterOperator.After,
                FilterOperator.Before,
                FilterOperator.Between
            };

            public FilterOptionDateTimeValidator()
            {
                Include(new FilterOptionDefaultValidator());
                RuleFor(x => x.Operator).Must(x => _validOperators.Contains(x!)).WithMessage("DateTime Filter Validation failed as {PropertyName} '{PropertyValue}' is not an accepted {PropertyName}.");
                RuleFor(x => x.Values).Must(x => DateTime.TryParse(x[0], CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime res))
                    .When(x => x.Operator != FilterOperator.Between).WithMessage("DateTime Filter Validation failed as {PropertyName} '{PropertyValue}' cannot be converted to DateTime.");
                RuleFor(x => x.Values).Must(x => x.Count == 2)
                    .When(x => x.Operator == FilterOperator.Between).WithMessage("DateTime Filter Validation failed as only one {PropertyName} '{PropertyValue}' was provided for Between Operator.");
                RuleFor(x => x.Values).Must(y => MultipleValueValidators.GenericCheckValidType<DateTime>(y, 2))
                    .When(x => x.Operator == FilterOperator.Between).WithMessage("DateTime Filter Validation failed as either one or both of the {PropertyName}s '{PropertyValue}' cannot be converted to DateTime.");
            }
        }

        sealed class FilterOptionGuidValidator : AbstractValidator<FilterCondition>
        {
            private static readonly List<FilterOperator> _validOperators = new()
            {
                FilterOperator.Equals,
                FilterOperator.In
            };

            public FilterOptionGuidValidator()
            {
                Include(new FilterOptionDefaultValidator());
                RuleFor(x => x.Operator).Must(x => _validOperators.Contains(x!)).WithMessage("Guid Filter Validation failed as {PropertyName} '{PropertyValue}' is not an accepted {PropertyName}.");
                RuleFor(x => x.Values).Must(x => Guid.TryParse(x[0], out Guid res))
                    .When(x => x.Operator == FilterOperator.Equals).WithMessage("Guid Filter Validation failed as {PropertyName} '{PropertyValue}' cannot be converted to Guid.");
                RuleFor(x => x.Values).Must(y => MultipleValueValidators.GenericCheckValidType<Guid>(y))
                    .When(x => x.Operator == FilterOperator.In).WithMessage("Guid Filter Validation failed as one or more of the {PropertyName}s '{PropertyValue}' cannot be converted to Guid.");

            }
        }

        sealed class FilterOptionEnumValidator : AbstractValidator<FilterCondition>
        {
            private static readonly List<FilterOperator> _validOperators = new()
            {
                FilterOperator.Equals,
                FilterOperator.NotEquals,
                FilterOperator.NotIn,
                FilterOperator.In
            };

            public FilterOptionEnumValidator()
            {
                Include(new FilterOptionDefaultValidator());
                RuleFor(x => x.Operator).Must(x => _validOperators.Contains(x!)).WithMessage("Enum Filter Validation failed as {PropertyName} '{PropertyValue}' is not an accepted {PropertyName}.");
            }
        }

        #endregion

        #region Fluent List Validation

        private static class MultipleValueValidators
        {
            public static bool ValidateValueLengthsAndCount(List<string> value, int? maxNumberOfCharactersPerValue = null, int? maxNumberOfValues = null)
            {
                if (maxNumberOfValues != null && value.Count > maxNumberOfValues) { return false; }

                foreach (string userValue in value)
                {
                    if (userValue.Length > maxNumberOfCharactersPerValue) { return false; }
                }

                return true;
            }

            public static bool ValidateCharactersProvided(string value)
            {
                Regex regex = new("[!=]"); // Minor validatoin to check that the following characters are not present in the user value: !=
                return !regex.IsMatch(value);
            }

            public static bool GenericCheckValidType<T>(List<string> values, int? expectedLength = null)
            {
                StringGenericTypeValidator<T> validator = new();
                return GenericBetweenValidate(validator, [.. values], expectedLength);
            }

            public static bool GenericBetweenValidate(AbstractValidator<string> validator, string[] args, int? expectedLength)
            {
                bool isValid = true;

                if (expectedLength != null && args.Length != expectedLength) { return false; }

                foreach (var arg in args)
                {
                    bool argIsValid = validator.Validate(arg).IsValid;
                    if (!argIsValid) isValid = false;
                }
                return isValid;
            }
        }
        #endregion

        #region Helpers
        sealed class StringGenericTypeValidator<T> : AbstractValidator<string>
        {
            public StringGenericTypeValidator()
            {
                RuleFor(x => x).Must(x => TryParseType<T>(x));
            }
        }

        private static bool TryParseType<T>(string value) => Type.GetTypeCode(typeof(T)) switch
        {
            TypeCode.Int64 => int.TryParse(value, out _),
            TypeCode.Int32 => int.TryParse(value, out _),
            TypeCode.Decimal => decimal.TryParse(value, out _),
            TypeCode.Double => double.TryParse(value, out _),
            TypeCode.DateTime => DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out _),
            TypeCode.Object => Guid.TryParse(value, out _),
            _ => false
        };
        #endregion
    }
}
