import type { FilterRequest } from './FilterRequest';

export interface TasksQuery {
    pageNumber?: number;
    pageSize?: number;
    sortColumn?: string;
    isDescending?: boolean;
    filters?: FilterRequest[];
};