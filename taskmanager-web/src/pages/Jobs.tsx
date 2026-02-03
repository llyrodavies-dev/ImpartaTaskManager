import { useState, useEffect } from 'react';
import { api } from '../services/ApiService';
import { useApiError } from '../hooks/useApiError';
import { useAuthStorage } from '../hooks/useAuthStorage';
import { useNavigate } from 'react-router-dom';
import { JobStatusLabels } from '../models/enums/JobStatusLabels';
import type { JobsPagedResponse } from '../models/JobsPagedResponse';
import CreateEditJobModal from '../components/modals/CreateEditJobModal';
import type { CreateJobCommand } from '../models/CreateJobCommand';
import type { UpdateJobRequest } from '../models/UpdateJobRequest';
import type { UpdateTaskCommand } from '../models/UpdateTaskCommand';

export default function Jobs() {
    const { errorTitle, validationErrors, handleApiResponse } = useApiError();
    const { getAuth } = useAuthStorage();
    const [jobsResponse, setJobsResponse] = useState<JobsPagedResponse | null>(null);
    const [editJob, setEditJob] = useState<{ id: string; title: string; description: string } | null>(null);
    const [editLoading, setEditLoading] = useState(false);
    const navigate = useNavigate();

    useEffect(() => {
        fetchJobs();
    }, []);

    const fetchJobs = async () => {
        const auth = getAuth();
        const token = auth.token;
        if (!token) return;
        const result = await api.get<JobsPagedResponse>('jobs', {
            headers: {
                Authorization: `Bearer ${token}`,
            },
        });
        if (handleApiResponse(result)) return;
        setJobsResponse(result);
    };

    const handleEditJob = async (data: UpdateTaskCommand) => {
            setEditLoading(true);
            const auth = getAuth();
            const token = auth.token;
            try {
                await api.put<UpdateJobRequest>(`tasks/${data.id}`, data, {
                    headers: { Authorization: `Bearer ${token}` }
                });
                setEditJob(null);
                fetchJobs();
            } finally {
                setEditLoading(false);
            }
        };
    
        const handleCreateJob = async (data: CreateJobCommand) => {
            setEditLoading(true);
            const auth = getAuth();
            const token = auth.token;
            try {
                await api.post<CreateJobCommand>(`jobs`, data, {
                    headers: { Authorization: `Bearer ${token}` }
                });
                setEditJob(null);
                fetchJobs();
            } finally {
                setEditLoading(false);
            }
        };

    const handleDelete = async (jobId: number) => {
        if (!window.confirm('Are you sure you want to delete this job?')) return;
        const auth = getAuth();
        const token = auth.token;
        try {
            await api.delete(`jobs/${jobId}`, {
                headers: {
                    Authorization: `Bearer ${token}`,
                },
            });
            fetchJobs(); // Refresh list after delete
        } catch (err) {
            // Optionally handle error
        }
    };

    return (
        <div className="p-8">
            <div className="flex items-center justify-between mb-4"
                style={{ paddingLeft:'20px', paddingRight:'20px' }}>
                <h1 className="text-2xl font-bold text-blue-800">Jobs</h1>
                <button
                    className="bg-green-700 hover:bg-green-800 text-white font-semibold px-5 py-2 rounded shadow transition"
                    onClick={() => setEditJob({ id: '', title: '', description: '' })}
                >
                    + Add New Job
                </button>
            </div>
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
                                className="py-3 px-4 font-semibold border-b text-blue-800"
                                style={{
                                    borderColor: 'var(--color-grey-blue-1)',
                                    borderTopLeftRadius: '0.75rem',
                                }}
                            >
                                Status
                                </th>
                                <th className="py-3 px-4 font-semibold border-b text-blue-800" style={{ borderColor: 'var(--color-grey-blue-1)' }}>
                                    Title
                                </th>
                                <th className="py-3 px-4 font-semibold border-b text-blue-800" style={{ borderColor: 'var(--color-grey-blue-1)' }}>
                                    Tasks
                                </th>
                                <th
                                    className="py-3 px-4 font-semibold border-b text-blue-800"
                                    style={{
                                        borderColor: 'var(--color-grey-blue-1)',
                                        borderTopRightRadius: '0.75rem',
                                    }}
                                >
                                Options
                            </th>
                        </tr>
                    </thead>
                    <tbody>
                        {jobsResponse?.items.map((job) => (
                            <tr key={job.id} className="hover:bg-gray-50">
                                <td className="py-2 px-4 border-b" style={{ borderColor: 'var(--color-grey-blue-1)' }}>
                                    {JobStatusLabels[job.status] ?? job.status}
                                </td>
                                <td className="py-2 px-4 border-b" style={{ borderColor: 'var(--color-grey-blue-1)' }}>
                                    {job.title}
                                </td>
                                <td className="py-2 px-4 border-b" style={{ borderColor: 'var(--color-grey-blue-1)' }}>
                                    {job.tasksCount ?? 0}
                                </td>
                                <td className="py-2 px-4 border-b flex gap-2 justify-center" style={{ borderColor: 'var(--color-grey-blue-1)' }}>
                                    <button
                                        className="bg-blue-700 text-white px-3 py-1 rounded hover:bg-blue-800 text-sm"
                                        onClick={() => navigate(`/jobs/${job.id}`)}
                                        title="View"
                                    >
                                        View
                                    </button>
                                    {/* <button
                                        className="bg-yellow-500 text-white px-3 py-1 rounded hover:bg-yellow-600 text-sm"
                                        onClick={() => handleEdit(job.id)}
                                        title="Edit"
                                    >
                                        Edit
                                    </button> */}
                                    <button
                                        className="bg-red-700 text-white px-3 py-1 rounded hover:bg-red-800 text-sm"
                                        // onClick={() => handleDelete(job.id)}
                                        title="Delete"
                                    >
                                        Delete
                                    </button>
                                </td>
                            </tr>
                        ))}
                    </tbody>
                </table>
                {jobsResponse && jobsResponse.totalCount === 0 && (
                    <div className="text-gray-500 text-center mt-8">No jobs found.</div>
                )}
            </div>
                        <CreateEditJobModal
                            editJob={editJob}
                            setEditJob={setEditJob}
                            onSave={editJob && !editJob.id ? handleCreateJob : handleEditJob}
                            editLoading={editLoading}
                        />
        </div>
    );
}