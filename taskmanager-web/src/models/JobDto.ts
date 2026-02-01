export interface JobDto {
  id: string;
  title: string;
  status: string; // or an enum if you define JobStatus
  createdAtUtc: string;
  createdBy: string;
  modifiedAtUtc?: string;
  modifiedBy?: string;
}
