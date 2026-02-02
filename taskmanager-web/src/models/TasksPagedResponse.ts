import type { TaskItemDto } from './TaskItemDto';

export interface TasksPagedResponse {
  items: TaskItemDto[];
  totalCount: number;
}