import type { JobDto } from './JobDto.ts';

export interface JobsPagedResponse {
  items: JobDto[];
  totalCount: number;
}