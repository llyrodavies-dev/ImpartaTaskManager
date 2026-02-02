namespace Utility.Filtering.Models
{
    public enum FilterOperator
    {
        None = 0,               // No operator
        Equals = 1,             // =
        NotEquals = 2,          // !=
        LessThan = 3,           // <
        LessThanOrEqual = 4,    // <=
        GreaterThan = 5,        // >
        GreaterThanOrEqual = 6, // >=
        In = 7,                 // IN
        NotIn = 8,              // NOT IN
        Before = 9,             // <=
        After = 10,             // >=
        Between = 11,           // BETWEEN
        // IsNull can be a expensive operation and should be used sparingly
        IsNull = 50,            // IS NULL
        // Contains is the most expensive operation, so it should be used sparingly
        // Contsains is the Last FilterOperator enum and will be implemented last in any query to improve performance
        Contains = 99           // CONTAINS    
    }
}
