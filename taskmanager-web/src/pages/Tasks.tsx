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
    const [query, setQuery] = useState<TasksQuery>({ pageNumber: 1, pageSize: 10 });
    const [editTask, setEditTask] = useState<{ id: string; title: string; description: string } | null>(null);
    const [editLoading, setEditLoading] = useState(false);
    const [expandedTaskId, setExpandedTaskId] = useState<string | null>(null);
    const [sortField, setSortField] = useState<string>('CreatedAtUtc');
    const [sortDirection, setSortDirection] = useState<'asc' | 'desc'>('desc');

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

    const totalPages = tasksResponse ? Math.ceil(tasksResponse.totalCount / (query.pageSize ?? 10)) : 1;
    const currentPage = query.pageNumber ?? 1;

    // Paging handlers
    const handlePrevPage = () => {
        if (currentPage > 1) setQuery(q => ({ ...q, pageNumber: currentPage - 1 }));
    };
    const handleNextPage = () => {
        if (currentPage < totalPages) setQuery(q => ({ ...q, pageNumber: currentPage + 1 }));
    };

    const handleSort = (field: string) => {
        setQuery(q => ({
            ...q,
            sortColumn: field,
            isDescending: q.sortColumn === field ? !q.isDescending : false,
            pageNumber: 1 // Reset to first page on sort
        }));
    };

    useEffect(() => {
        setQuery(q => ({
            ...q,
            sortField,
            sortDirection,
            pageNumber: 1 // Reset to first page on sort
        }));
    }, [sortField, sortDirection]);

    return(
        <div className="p-8">
            <div className="bg-white rounded-xl shadow-lg p-6 mb-4">
                <h1 className="text-2xl font-bold mb-4 text-blue-800 text-left main-text" style={{ paddingLeft: '20px' }}>
                    All Tasks
                </h1>
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
                            <tr style={{ background: 'var(--nav-background)' }}>
                                <th
                                    className="px-4 py-2 font-semibold border-b text-white cursor-pointer select-none"
                                    style={{ borderTopLeftRadius: '0.75rem', width: '120px' }}
                                    onClick={() => handleSort('Status')}
                                >
                                    Status
                                    <span className="ml-2">
                                        <span className={query.sortColumn === 'Status' && !query.isDescending ? 'text-yellow-300 font-bold' : 'text-white'}>
                                            ▲
                                        </span>
                                        <span className={query.sortColumn === 'Status' && query.isDescending ? 'text-yellow-300 font-bold' : 'text-white'}>
                                            ▼
                                        </span>
                                    </span>
                                </th>
                                <th
                                    className="px-4 py-2 font-semibold border-b text-white cursor-pointer select-none"
                                    style={{ width: '180px' }}
                                    onClick={() => handleSort('Title')}
                                >
                                    Title
                                    <span className="ml-2">
                                        <span className={query.sortColumn === 'Title' && !query.isDescending ? 'text-yellow-300 font-bold' : 'text-white'}>
                                            ▲
                                        </span>
                                        <span className={query.sortColumn === 'Title' && query.isDescending ? 'text-yellow-300 font-bold' : 'text-white'}>
                                            ▼
                                        </span>
                                    </span>
                                </th>
                                <th
                                    className="px-4 py-2 font-semibold border-b text-white cursor-pointer select-none"
                                    style={{ maxWidth: '500px', width: '500px' }}
                                    onClick={() => handleSort('Description')}
                                >
                                    Description
                                    <span className="ml-2">
                                        <span className={query.sortColumn === 'Description' && !query.isDescending ? 'text-yellow-300 font-bold' : 'text-white'}>
                                            ▲
                                        </span>
                                        <span className={query.sortColumn === 'Description' && query.isDescending ? 'text-yellow-300 font-bold' : 'text-white'}>
                                            ▼
                                        </span>
                                    </span>
                                </th>
                                <th
                                    className="px-4 py-2 font-semibold border-b text-white cursor-pointer select-none"
                                    style={{ width: '180px' }}
                                    onClick={() => handleSort('CreatedAtUtc')}
                                >
                                    Created At
                                    <span className="ml-2">
                                        <span className={query.sortColumn === 'CreatedAtUtc' && !query.isDescending ? 'text-yellow-300 font-bold' : 'text-white'}>
                                            ▲
                                        </span>
                                        <span className={query.sortColumn === 'CreatedAtUtc' && query.isDescending ? 'text-yellow-300 font-bold' : 'text-white'}>
                                            ▼
                                        </span>
                                    </span>
                                </th>
                                <th className="px-4 py-2 font-semibold border-b text-white" style={{ borderTopRightRadius: '0.75rem', width: '180px' }}>Options</th>
                            </tr>
                        </thead>
                        <tbody>
                            {tasksResponse?.items.map((task, idx) => {
                                const isExpanded = expandedTaskId === task.id;
                                const isLastRow = idx === tasksResponse.items.length - 1;
                                return (
                                    <tr
                                        key={task.id}
                                        className="hover:bg-gray-50 cursor-pointer"
                                        onClick={() => setExpandedTaskId(isExpanded ? null : task.id)}
                                    >
                                        <td
                                            className="px-4 py-2 border-b row-bg"
                                            style={{
                                                borderColor: 'var(--color-grey-blue-1)',
                                                ...(isLastRow && { borderBottomLeftRadius: '0.75rem' }),
                                                width: '120px',
                                            }}
                                        >
                                            {TaskItemStatusLabels[task.status] ?? task.status}
                                        </td>
                                        <td className="px-4 py-2 border-b row-bg" style={{ borderColor: 'var(--color-grey-blue-1)', width: '180px' }}>
                                            {task.title}
                                        </td>
                                        <td
                                            className="px-4 py-2 border-b row-bg"
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
                                        <td className="px-4 py-2 border-b row-bg"
                                            style={{
                                                borderColor: 'var(--color-grey-blue-1)',
                                                width: '180px'
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
                                            className="py-2 px-4 border-b row-bg"
                                            style={{
                                                borderColor: 'var(--color-grey-blue-1)',
                                                ...(isLastRow && { borderBottomRightRadius: '0.75rem' }),
                                                width: '180px'
                                            }}
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
                {/* Paging controls INSIDE the card, just under the table */}
                {tasksResponse  && (
                    <div className="flex items-center justify-between mt-6 px-2">
                        <span className="text-gray-700 font-semibold">
                            Total Tasks: {tasksResponse ? tasksResponse.totalCount : 0}
                        </span>
                        <div className="flex items-center gap-4">
                            <button
                                className="px-4 py-2 rounded bg-blue-100 text-blue-700 font-semibold disabled:opacity-50"
                                onClick={handlePrevPage}
                                disabled={currentPage === 1 || !tasksResponse || tasksResponse.totalCount === 0}
                            >
                                Previous
                            </button>
                            <span className="text-gray-700 font-semibold">
                                Page {currentPage} of {totalPages}
                            </span>
                            <button
                                className="px-4 py-2 rounded bg-blue-100 text-blue-700 font-semibold disabled:opacity-50"
                                onClick={handleNextPage}
                                disabled={currentPage === totalPages || !tasksResponse || tasksResponse.totalCount === 0}
                            >
                                Next
                            </button>
                        </div>
                    </div>
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