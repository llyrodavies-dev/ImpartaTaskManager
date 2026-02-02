export interface TaskItemDto {
    id: string;
    JobId: string;
    title: string;
    description: string;
    status: number;
    createdAtUtc: string;
    createdBy: string;
    modifiedAtUtc?: string;
    modifiedBy?: string;
}