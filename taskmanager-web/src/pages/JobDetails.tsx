import { useState, useEffect } from 'react';
import { api } from '../services/ApiService';
import { useApiError } from '../hooks/useApiError';
import { useAuthStorage } from '../hooks/useAuthStorage';
import { useParams, useNavigate } from 'react-router-dom';
import type { JobDto } from '../models/JobDto';
import { jobStatusLabels } from '../models/enums/JobStatusLabels';
import { taskItemStatusLabels } from '../models/enums/TaskItemStatusLabels';


export default function JobDetails() {
    const { errorTitle, validationErrors, handleApiResponse } = useApiError();
    const { getAuth } = useAuthStorage();
    const { id } = useParams<{ id: string }>();
    const [jobResponse, setJobResponse] = useState<JobDto | null>(null);

    useEffect(() => {
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
        fetchJobs();
    }, []);

    return(
        <div className="p-8">
            <h1 className="text-2xl font-bold mb-4">Job Details</h1>
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
                <div className="mb-8 bg-white rounded-lg shadow-md p-6">
                    <div className="mb-2"><span className="font-semibold">Title:</span> {jobResponse.title}</div>
                    <div className="mb-2"><span className="font-semibold">Status:</span> {jobStatusLabels[jobResponse.status] ?? jobResponse.status}</div>
                    <div className="mb-2"><span className="font-semibold">Created At:</span> {new Date(jobResponse.createdAtUtc).toLocaleString()}</div>
                    <div className="mb-2"><span className="font-semibold">Created By:</span> {jobResponse.createdBy}</div>
                    {jobResponse.modifiedAtUtc && (
                        <div className="mb-2"><span className="font-semibold">Modified At:</span> {new Date(jobResponse.modifiedAtUtc).toLocaleString()}</div>
                    )}
                    {jobResponse.modifiedBy && (
                        <div className="mb-2"><span className="font-semibold">Modified By:</span> {jobResponse.modifiedBy}</div>
                    )}
                </div>
            )}

            <h2 className="text-xl font-bold mb-4">Tasks</h2>
            {jobResponse?.tasks && jobResponse.tasks.length > 0 ? (
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
                            {jobResponse.tasks.map(task => (
                                <tr key={task.id} className="border-t">
                                    <td className="px-4 py-2">{task.title}</td>
                                    <td className="px-4 py-2">{task.description}</td>
                                    <td className="px-4 py-2">{taskItemStatusLabels[task.status] ?? task.status}</td>
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