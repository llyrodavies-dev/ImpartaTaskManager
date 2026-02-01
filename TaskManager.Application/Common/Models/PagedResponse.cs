namespace TaskManager.Application.Common.Models
{
    public class PagedResponse<T>
    {
        public List<T> Items { get; set; } = [];
        public int TotalCount { get; set; }
    }
}
