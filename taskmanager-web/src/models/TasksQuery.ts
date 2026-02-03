import type { FilterRequest } from './FilterRequest';

export interface TasksQuery {
    page?: number;
    pageSize?: number;
    sortColumn?: string;
    isDescending?: boolean;
    filters?: FilterRequest[];
};