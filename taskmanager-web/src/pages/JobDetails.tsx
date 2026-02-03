import { useState, useEffect } from 'react';
import { api } from '../services/ApiService';
import { useApiError } from '../hooks/useApiError';
import { useAuthStorage } from '../hooks/useAuthStorage';
import { useParams, useNavigate } from 'react-router-dom';
import { JobStatusLabels } from '../models/enums/JobStatusLabels';
import { TaskItemStatusLabels } from '../models/enums/TaskItemStatusLabels';
import EditTaskModal from '../components/modals/EditTaskModal';
import DeleteModal from '../components/modals/DeleteModal';
import type { JobDto } from '../models/JobDto';
import type { UpdateTaskCommand } from '../models/UpdateTaskCommand';
import type { CreateTaskCommand } from '../models/CreateTaskCommand';


export default function JobDetails() {
    const [jobResponse, setJobResponse] = useState<JobDto | null>(null);
    const [editTask, setEditTask] = useState<{ id: string; title: string; description: string } | null>(null);
    const [editLoading, setEditLoading] = useState(false);
    const [expandedTaskId, setExpandedTaskId] = useState<string | null>(null);
    const [deleteTaskId, setDeleteTaskId] = useState<string | null>(null);
    const [deleteLoading, setDeleteLoading] = useState(false);

    const { errorTitle, validationErrors, handleApiResponse } = useApiError();
    const { getAuth } = useAuthStorage();
    const { id } = useParams<{ id: string }>();

    useEffect(() => {
        fetchJobs();
    }, []);

    const fetchJobs = async () => {
            const auth = getAuth();
            const token = auth.token;
            if (!token) return;
            const result = await api.get<JobDto>(`jobs/${id}`, {
                headers: {
                    Authorization: `Bearer ${token}`,
                },
            });
            if (handleApiResponse(result)) return;
            setJobResponse(result);
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
            fetchJobs();
        } finally {
            setEditLoading(false);
        }
    };

    const handleCreateTask = async (data: CreateTaskCommand) => {
        setEditLoading(true);
        const auth = getAuth();
        const token = auth.token;
        try {
            await api.post<CreateTaskCommand>(`jobs/${id}/tasks`, data, {
                headers: { Authorization: `Bearer ${token}` }
            });
            setEditTask(null);
            fetchJobs();
        } finally {
            setEditLoading(false);
        }
    };

    const handleDelete = async (taskId: string) => {
        setDeleteLoading(true);
        const auth = getAuth();
        const token = auth.token;
        try {
            await api.delete(`tasks/${taskId}`, {
                headers: {
                    Authorization: `Bearer ${token}`,
                },
            });
            fetchJobs();
        } finally {
            setDeleteLoading(false);
            setDeleteTaskId(null);
        }
    };

    return(
        <div className="p-8">
            <h1 className="text-2xl font-bold mb-4 text-blue-800 text-left" style={{paddingLeft: '20px'}}>Job Details</h1>
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

            {jobResponse && (
                <div className="mb-8 bg-white rounded-xl shadow-lg p-8 max-w-2xl">
                    <div className="mb-4 flex items-center gap-2">
                        <div className="font-semibold text-blue-800">Title:</div>
                        <div className="text-gray-900">{jobResponse.title}</div>
                    </div>
                    <div className="mb-4 flex items-center gap-2">
                        <div className="font-semibold text-blue-800">Status:</div>
                        <div className="text-gray-900">{JobStatusLabels[jobResponse.status] ?? jobResponse.status}</div>
                    </div>
                    <div className="mb-4 flex items-center gap-2">
                        <div className="font-semibold text-blue-800">Created At:</div>
                        <div className="text-gray-900">{new Date(jobResponse.createdAtUtc).toLocaleString('en-GB')}</div>
                    </div>
                </div>
                )}
            <div className="flex items-center justify-between mb-4"
                style={{ paddingLeft:'20px', paddingRight:'20px' }}>
                <h1 className="text-2xl font-bold text-blue-800">Job Tasks</h1>
                <button
                    className="bg-green-700 hover:bg-green-800 text-white font-semibold px-5 py-2 rounded shadow transition"
                    onClick={() => setEditTask({ id: '', title: '', description: '' })}
                >
                    + Add New Task
                </button>
            </div>
            {jobResponse?.tasks && jobResponse.tasks.length > 0 ? (
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
                                <th className="px-4 py-2 border-b text-blue-800" style={{ borderColor: 'var(--color-grey-blue-1)', borderTopLeftRadius: '0.75rem' }}>Status</th>
                                <th className="px-4 py-2 border-b text-blue-800" style={{ borderColor: 'var(--color-grey-blue-1)' }}>Title</th>
                                <th className="px-4 py-2 border-b text-blue-800" style={{ borderColor: 'var(--color-grey-blue-1)' }}>Description</th>
                                <th className="px-4 py-2 border-b text-blue-800" style={{ borderColor: 'var(--color-grey-blue-1)' }}>Created At</th>
                                <th className="px-4 py-2 border-b text-blue-800" style={{ borderColor: 'var(--color-grey-blue-1)', borderTopRightRadius: '0.75rem'  }}>Options</th>
                            </tr>
                        </thead>
                        <tbody>
                            {jobResponse.tasks.map((task, idx) => {
                                const isExpanded = expandedTaskId === task.id;
                                const isLastRow = idx === jobResponse.tasks!.length - 1;
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
                                            }}
                                        >
                                            {TaskItemStatusLabels[task.status] ?? task.status}
                                        </td>
                                        <td className="px-4 py-2 border-b" style={{ borderColor: 'var(--color-grey-blue-1)' }}>
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
                                        <td className="px-4 py-2 border-b" style={{ borderColor: 'var(--color-grey-blue-1)' }}>
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
                                            }}
                                        >
                                            <div className="flex gap-2 justify-center">
                                                <select
                                                    className="border rounded px-2 py-1 text-sm"
                                                    value={task.status}
                                                    onClick={e => e.stopPropagation()}
                                                    onChange={async (e) => {
                                                        const newStatus = e.target.value;
                                                        const auth = getAuth();
                                                        const token = auth.token;

                                                        await api.patch(`tasks/${task.id}/status`, { status: newStatus }, {
                                                            headers: { Authorization: `Bearer ${token}` }
                                                        });
                                                        fetchJobs()
                                                    }}>
                                                    {Object.entries(TaskItemStatusLabels).map(([status, label]) => (
                                                        <option key={status} value={status}>{label}</option>
                                                    ))}
                                                </select>
                                                <button className="bg-blue-700 text-white px-3 py-1 rounded hover:bg-blue-800 text-sm"
                                                    onClick={e => {
                                                        e.stopPropagation();
                                                        setEditTask({ id: task.id, title: task.title, description: task.description });
                                                    }}
                                                    title="Edit">
                                                    Edit
                                                </button>
                                                <button className="bg-red-700 text-white px-3 py-1 rounded hover:bg-red-800 text-sm"
                                                    onClick={e => {
                                                        e.stopPropagation();
                                                        setDeleteTaskId(task.id); // <-- Open the modal instead of deleting immediately
                                                    }}
                                                    title="Delete">
                                                    Delete
                                                </button>
                                            </div>
                                        </td>
                                    </tr>
                                );
                            })}
                        </tbody>
                    </table>
                </div>
            ) : (
                <div className="text-gray-500 text-center mt-8">No tasks found for this job.</div>
            )}
            <EditTaskModal
                editTask={editTask}
                setEditTask={setEditTask}
                onSave={editTask && !editTask.id ? handleCreateTask : handleEditTask}
                editLoading={editLoading}
            />
            <DeleteModal
                open={deleteTaskId !== null}
                itemName={jobResponse?.tasks?.find(t => t.id === deleteTaskId)?.title}
                onConfirm={() => deleteTaskId && handleDelete(deleteTaskId)}
                onCancel={() => setDeleteTaskId(null)}
                loading={deleteLoading}
            />
        </div>
    );
}