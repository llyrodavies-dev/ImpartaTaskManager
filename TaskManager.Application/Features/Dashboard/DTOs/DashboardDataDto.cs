namespace TaskManager.Application.Features.Dashboard.DTOs
{
    public record DashboardDataDto(
         JobSummaryDto JobSummary,
         TaskSummaryDto TaskSummary,
         ProgressDto Progress
     );

    public record JobSummaryDto(
        int Total,
        int Active,
        int Completed
    );

    public record TaskSummaryDto(
        int Total,
        int NotStarted,
        int InProgress,
        int Blocked,
        int Completed
    );

    public record ProgressDto(
        int OverallPercent
    );
}
