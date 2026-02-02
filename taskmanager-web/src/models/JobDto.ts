import type { TaskItemDto } from "./TaskItemDto";

export interface JobDto {
  id: string;
  title: string;
  status: number; 
  createdAtUtc: string;
  createdBy: string;
  modifiedAtUtc?: string;
  modifiedBy?: string;
  tasks?: TaskItemDto[]; 
}
