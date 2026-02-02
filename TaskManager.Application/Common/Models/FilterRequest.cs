using Utility.Filtering.Models;

namespace TaskManager.Application.Common.Models
{
    public class FilterRequest
    {
        public List<FilterCondition> Filter { get; set; } = [];
        public string SortColumn { get; set; } = string.Empty;
        public bool IsDescending { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
