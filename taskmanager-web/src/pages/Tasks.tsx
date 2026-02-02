import { useState, useEffect } from 'react';
import { api } from '../services/ApiService';
import { useApiError } from '../hooks/useApiError';
import { useAuthStorage } from '../hooks/useAuthStorage';
import type { TasksPagedResponse } from '../models/TasksPagedResponse';
import type { FilterRequest } from '../models/FilterRequest';
import { TaskItemStatusLabels } from '../models/enums/TaskItemStatusLabels';
import TaskFilterForm from '../components/TaskFilterForm';

type TasksQuery = {
    page?: number;
    pageSize?: number;
    status?: string;
    filters?: FilterRequest[];
};

const FIELD_OPTIONS = [
    { value: 'Title', label: 'Title' },
    { value: 'Status', label: 'Status' },
    { value: 'CreatedBy', label: 'Created By' },
    // Add more fields as needed for this page
];

const FIELD_OPERATORS = {
    Title: [
        { value: 1, label: 'Equals' },
        { value: 99, label: 'Contains' },
    ],
    Status: [
        { value: 1, label: 'Equals' },
    ],
};

export default function Tasks(){
    const { errorTitle, validationErrors, handleApiResponse } = useApiError();
    const { getAuth } = useAuthStorage();
    const [tasksResponse, setTasksResponse] = useState<TasksPagedResponse | null>(null);
    const [query, setQuery] = useState<TasksQuery>({ page: 1, pageSize: 10 });

    const handleAddFilter = (filter: FilterRequest) => {
        setQuery(q => ({
            ...q,
            filters: [...(q.filters || []), filter],
        }));
    };

    const handleDeleteFilter = (index: number) => {
        setQuery(q => ({
            ...q,
            filters: (q.filters || []).filter((_, i) => i !== index),
        }));
    };

    useEffect(() => {
        const fetchTasks = async () => {
            const auth = getAuth();
            const token = auth.token;
            if (!token) return;
            console.log('Sending query:', query);
            const result = await api.post<TasksPagedResponse>('tasks/search', query, {
                headers: {
                    Authorization: `Bearer ${token}`,

                },
            });
            if (handleApiResponse(result)) return;
            setTasksResponse(result);
        };
        fetchTasks();
    }, [query]);

    return(
        <div className="p-8">
            <h1 className="text-2xl font-bold mb-4">Tasks</h1>

            <TaskFilterForm
                fieldOptions={FIELD_OPTIONS}
                fieldOperators={FIELD_OPERATORS}
                filters={query.filters || []}
                onAddFilter={handleAddFilter}
                onDeleteFilter={handleDeleteFilter}
            />
            {errorTitle && (
                <div className="mb-4 text-red-600 text-center font-semibold">{errorTitle}</div>
            )}
            {validationErrors && (
                <ul className="mb-4 text-red-500 text-sm list-disc list-inside">
                    {Object.entries(validationErrors).map(([field, messages]) =>
                        messages.map((msg, i) => (
                            <li key={field + i}>{field}: {msg}</li>
                        ))
                    )}
                </ul>
            )}
                {tasksResponse?.items && tasksResponse.items.length > 0 ? (
                    <div className="overflow-x-auto">
                        <table className="min-w-full bg-white rounded shadow">
                            <thead>
                                <tr>
                                    <th className="px-4 py-2 text-left">Title</th>
                                    <th className="px-4 py-2 text-left">Description</th>
                                    <th className="px-4 py-2 text-left">Status</th>
                                    <th className="px-4 py-2 text-left">Created At</th>
                                    <th className="px-4 py-2 text-left">Created By</th>
                                    <th className="px-4 py-2 text-left">Modified At</th>
                                    <th className="px-4 py-2 text-left">Modified By</th>
                                </tr>
                            </thead>
                            <tbody>
                                {tasksResponse.items.map(task => (
                                    <tr key={task.id} className="border-t">
                                        <td className="px-4 py-2">{task.title}</td>
                                        <td className="px-4 py-2">{task.description}</td>
                                        <td className="px-4 py-2">{TaskItemStatusLabels[task.status] ?? task.status}</td>
                                        <td className="px-4 py-2">{new Date(task.createdAtUtc).toLocaleString()}</td>
                                        <td className="px-4 py-2">{task.createdBy}</td>
                                        <td className="px-4 py-2">{task.modifiedAtUtc ? new Date(task.modifiedAtUtc).toLocaleString() : '-'}</td>
                                        <td className="px-4 py-2">{task.modifiedBy ?? '-'}</td>
                                    </tr>
                                ))}
                            </tbody>
                        </table>
                    </div>
                ) : (
                    <div className="text-gray-500 text-center mt-8">No tasks found for this job.</div>
                )}
        </div>
    );
}