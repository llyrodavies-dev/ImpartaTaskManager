import { useState, useEffect } from 'react';
import { api } from '../services/ApiService';
import { useApiError } from '../hooks/useApiError';
import { useAuthStorage } from '../hooks/useAuthStorage';
import { TaskItemStatusLabels } from '../models/enums/TaskItemStatusLabels';
import type { TasksPagedResponse } from '../models/TasksPagedResponse';
import type { FilterRequest } from '../models/FilterRequest';
import type { UpdateTaskCommand } from '../models/UpdateTaskCommand';
import TaskFilterForm from '../components/TaskFilterForm';
import EditTaskModal from '../components/modals/EditTaskModal';
import type { TasksQuery } from '../models/TasksQuery';

const FIELD_OPTIONS = [
    { value: 'Title', label: 'Title' },
    { value: 'Description', label: 'Description' },
    { value: 'Status', label: 'Status' },
];

const FIELD_OPERATORS = {
    Title: [
        { value: 99, label: 'Contains' },
    ],
    Description: [
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
    const [editTask, setEditTask] = useState<{ id: string; title: string; description: string } | null>(null);
    const [editLoading, setEditLoading] = useState(false);
    const [expandedTaskId, setExpandedTaskId] = useState<string | null>(null);

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
        fetchTasks();
    }, [query]);

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

        const handleEditTask = async (data: UpdateTaskCommand) => {
            setEditLoading(true);
            const auth = getAuth();
            const token = auth.token;
            try {
                await api.put<UpdateTaskCommand>(`tasks/${data.id}`, data, {
                    headers: { Authorization: `Bearer ${token}` }
                });
                setEditTask(null);
                fetchTasks();
            } finally {
                setEditLoading(false);
            }
        };
    
        const handleDelete = async (taskId: string) => {
            if (!window.confirm('Are you sure you want to delete this task?')) return;
            const auth = getAuth();
            const token = auth.token;
                await api.delete(`tasks/${taskId}`, {
                    headers: {
                        Authorization: `Bearer ${token}`,
                    },
                });
                fetchTasks();
        };

    return(
        <div className="p-8">
            <h1 className="text-2xl font-bold mb-4 text-blue-800 text-left" style={{paddingLeft: '20px'}}>All Tasks</h1>
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
            <div className="overflow-x-auto">
                <table
                    className="min-w-full rounded-xl border-separate"
                    style={{
                        background: 'var(--color-white-1)',
                        borderCollapse: 'separate',
                        borderSpacing: 0,
                    }}
                >
                    <thead>
                        <tr style={{ background: '#e3eaf2' }}>
                            <th
                                className="py-3 px-4 font-semibold border-b text-blue-800 select-none cursor-pointer"
                                style={{
                                    borderColor: 'var(--color-grey-blue-1)',
                                    borderTopLeftRadius: '0.75rem',
                                    width: '120px',
                                }}
                                onClick={() => {
                                    setQuery(q => ({
                                        ...q,
                                        sortColumn: 'Status',
                                        isDescending: q.sortColumn === 'Status' ? !q.isDescending : false,
                                    }));
                                }}
                            >
                                <span className="flex items-center gap-1">
                                    Status
                                    <span className="flex flex-col ml-1 text-xs leading-none">
                                        <span className={query.sortColumn === 'Status' && !query.isDescending ? "text-blue-700 font-bold" : "text-gray-400"}>
                                            ▲
                                        </span>
                                        <span className={query.sortColumn === 'Status' && query.isDescending ? "text-blue-700 font-bold" : "text-gray-400"}>
                                            ▼
                                        </span>
                                    </span>
                                </span>
                            </th>
                            <th
                                className="py-3 px-4 font-semibold border-b text-blue-800 select-none cursor-pointer"
                                style={{
                                    borderColor: 'var(--color-grey-blue-1)',
                                    width: '180px',
                                }}
                                onClick={() => {
                                    setQuery(q => ({
                                        ...q,
                                        sortColumn: 'Title',
                                        isDescending: q.sortColumn === 'Title' ? !q.isDescending : false,
                                    }));
                                }}
                            >
                                <span className="flex items-center gap-1">
                                    Title
                                    <span className="flex flex-col ml-1 text-xs leading-none">
                                        <span className={query.sortColumn === 'Title' && !query.isDescending ? "text-blue-700 font-bold" : "text-gray-400"}>
                                            ▲
                                        </span>
                                        <span className={query.sortColumn === 'Title' && query.isDescending ? "text-blue-700 font-bold" : "text-gray-400"}>
                                            ▼
                                        </span>
                                    </span>
                                </span>
                            </th>
                            <th
                                className="py-3 px-4 font-semibold border-b text-blue-800 select-none cursor-pointer"
                                style={{
                                    borderColor: 'var(--color-grey-blue-1)',
                                    maxWidth: '500px',
                                    width: '500px',
                                }}
                                onClick={() => {
                                    setQuery(q => ({
                                        ...q,
                                        sortColumn: 'Description',
                                        isDescending: q.sortColumn === 'Description' ? !q.isDescending : false,
                                    }));
                                }}
                            >
                                <span className="flex items-center gap-1">
                                    Description
                                    <span className="flex flex-col ml-1 text-xs leading-none">
                                        <span className={query.sortColumn === 'Description' && !query.isDescending ? "text-blue-700 font-bold" : "text-gray-400"}>
                                            ▲
                                        </span>
                                        <span className={query.sortColumn === 'Description' && query.isDescending ? "text-blue-700 font-bold" : "text-gray-400"}>
                                            ▼
                                        </span>
                                    </span>
                                </span>
                            </th>
                            <th
                                className="py-3 px-4 font-semibold border-b text-blue-800 select-none cursor-pointer"
                                style={{
                                    borderColor: 'var(--color-grey-blue-1)',
                                    width: '180px',
                                }}
                                onClick={() => {
                                    setQuery(q => ({
                                        ...q,
                                        sortColumn: 'CreatedAt',
                                        isDescending: q.sortColumn === 'CreatedAt' ? !q.isDescending : false,
                                    }));
                                }}
                            >
                                <span className="flex items-center gap-1">
                                    Created At
                                    <span className="flex flex-col ml-1 text-xs leading-none">
                                        <span className={query.sortColumn === 'CreatedAt' && !query.isDescending ? "text-blue-700 font-bold" : "text-gray-400"}>
                                            ▲
                                        </span>
                                        <span className={query.sortColumn === 'CreatedAt' && query.isDescending ? "text-blue-700 font-bold" : "text-gray-400"}>
                                            ▼
                                        </span>
                                    </span>
                                </span>
                            </th>
                            <th
                                className="px-4 py-2 border-b text-blue-800"
                                style={{
                                    borderColor: 'var(--color-grey-blue-1)',
                                    borderTopRightRadius: '0.75rem',
                                    width: '180px',
                                }}
                            >
                                Options
                            </th>
                        </tr>
                    </thead>
                    <tbody>
                        {tasksResponse && tasksResponse.items.map((task, idx) => {
                            const isLastRow = idx === tasksResponse.items.length - 1;
                            const isExpanded = expandedTaskId === task.id;
                            return (
                                <tr
                                    key={task.id}
                                    className="hover:bg-gray-50 cursor-pointer"
                                    onClick={() => setExpandedTaskId(isExpanded ? null : task.id)}
                                >
                                    <td
                                        className="px-4 py-2 border-b"
                                        style={{
                                            borderColor: 'var(--color-grey-blue-1)',
                                            ...(isLastRow && { borderBottomLeftRadius: '0.75rem' }),
                                            width: '120px',
                                        }}
                                    >
                                        {TaskItemStatusLabels[task.status] ?? task.status}
                                    </td>
                                    <td
                                        className="px-4 py-2 border-b"
                                        style={{
                                            borderColor: 'var(--color-grey-blue-1)',
                                            width: '180px',
                                        }}
                                    >
                                        {task.title}
                                    </td>
                                    <td
                                        className="px-4 py-2 border-b"
                                        style={{
                                            borderColor: 'var(--color-grey-blue-1)',
                                            maxWidth: '500px',
                                            width: '500px',
                                            overflow: isExpanded ? 'visible' : 'hidden',
                                            textOverflow: isExpanded ? 'unset' : 'ellipsis',
                                            whiteSpace: isExpanded ? 'normal' : 'nowrap',
                                            overflowWrap: isExpanded ? 'break-word' : 'normal',
                                            wordBreak: isExpanded ? 'break-word' : 'normal',
                                        }}
                                        title={!isExpanded ? task.description : undefined}
                                    >
                                        {task.description}
                                    </td>
                                    <td
                                        className="px-4 py-2 border-b"
                                        style={{
                                            borderColor: 'var(--color-grey-blue-1)',
                                            width: '180px',
                                        }}
                                    >
                                        {new Date(task.createdAtUtc).toLocaleString('en-GB', {
                                            day: '2-digit',
                                            month: '2-digit',
                                            year: 'numeric',
                                            hour: 'numeric',
                                            minute: '2-digit',
                                            hour12: false
                                        }).replace(/\//g, '-')}
                                    </td>
                                    <td
                                        className="py-2 px-4 border-b"
                                        style={{
                                            borderColor: 'var(--color-grey-blue-1)',
                                            ...(isLastRow && { borderBottomRightRadius: '0.75rem' }),
                                            width: '180px',
                                        }}
                                        onClick={e => e.stopPropagation()} // Prevent row expand on options click
                                    >
                                        <div className="flex gap-2 justify-center">
                                            <select
                                                className="border rounded px-2 py-1 text-sm"
                                                value={task.status}
                                                disabled={task.status === 3} // Disable if status is Completed
                                                onChange={async (e) => {
                                                    const newStatus = e.target.value;
                                                    const auth = getAuth();
                                                    const token = auth.token;

                                                    await api.patch(`tasks/${task.id}/status`, { status: newStatus }, {
                                                        headers: { Authorization: `Bearer ${token}` }
                                                    });
                                                    fetchTasks()
                                                }}
                                                onClick={e => e.stopPropagation()}
                                            >
                                                {Object.entries(TaskItemStatusLabels)
                                                    .filter(([status]) => status !== "0") // Exclude "Unspecified"
                                                    .map(([status, label]) => (
                                                        <option key={status} value={status}>{label}</option>
                                                    ))}
                                            </select>
                                            <button
                                                className={`px-3 py-1 rounded text-sm
                                                    ${task.status === 3
                                                        ? "bg-gray-400 text-gray-200"
                                                        : "bg-blue-600 text-white hover:bg-blue-700"}
                                                `}
                                                disabled={task.status === 3}
                                                onClick={e => {
                                                    e.stopPropagation();
                                                    setEditTask({ id: task.id, title: task.title, description: task.description });
                                                }}
                                                title="Edit"
                                            >
                                                Edit
                                            </button>
                                            <button
                                                className="bg-red-600 text-white px-3 py-1 rounded hover:bg-red-700 text-sm"
                                                onClick={e => {
                                                    e.stopPropagation();
                                                    handleDelete(task.id);
                                                }}
                                                title="Delete"
                                            >
                                                Delete
                                            </button>
                                        </div>
                                    </td>
                                </tr>
                            );
                        })}
                    </tbody>
                </table>
                {tasksResponse && tasksResponse.totalCount === 0 && (
                    <div className="text-gray-500 text-center mt-8">No tasks found.</div>
                )}
            </div>
            <EditTaskModal
                            editTask={editTask}
                            setEditTask={setEditTask}
                            onSave={handleEditTask}
                            editLoading={editLoading}
                        />
        </div>
    );
}