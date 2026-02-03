export interface JobSummaryDto {
    total: number;
    active: number;
    completed: number;
}

export interface TaskSummaryDto {
    total: number;
    notStarted: number;
    inProgress: number;
    blocked: number;
    completed: number;
}

export interface ProgressDto {
    overallPercent: number;
}

export interface DashboardDataDto {
    jobSummary: JobSummaryDto;
    taskSummary: TaskSummaryDto;
    progress: ProgressDto;
}