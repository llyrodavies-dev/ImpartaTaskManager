namespace TaskManager.Domain.Enums
{
    public enum JobStatus
    {
        Unspecified = 0,    // this is a code default value
        NotStarted = 1,     // this should be the business logic default value
        InProgress = 2,
        Completed = 3,
        OnHold = 4,
        Blocked = 5,
        Cancelled = 6
    }
}
