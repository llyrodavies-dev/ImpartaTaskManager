import { useState, useEffect } from 'react';
import { api } from '../services/ApiService';
import { useApiError } from '../hooks/useApiError';
import { useAuthStorage } from '../hooks/useAuthStorage';
import { useNavigate } from 'react-router-dom';
import type { JobsPagedResponse } from '../models/JobsPagedResponse';
import type { JobDto } from '../models/JobDto';
import { JobStatusLabels } from '../models/enums/JobStatusLabels';


export default function Jobs() {
    const { errorTitle, validationErrors, handleApiResponse } = useApiError();
    const { getAuth } = useAuthStorage();
    const [jobsResponse, setJobsResponse] = useState<JobsPagedResponse | null>(null);
    const navigate = useNavigate();

    useEffect(() => {
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
        fetchJobs();
    }, []);

    return (
        <div className="p-8">
            <h1 className="text-2xl font-bold mb-4">Jobs</h1>
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
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
                {jobsResponse?.items.map((job: JobDto) => (
                    <div
                        key={job.id}
                        className="bg-white rounded-lg shadow-md p-6 flex flex-col justify-between hover:shadow-lg transition cursor-pointer"
                        onClick={() => navigate(`/jobs/${job.id}`)}
                        >
                        <div>
                            <h2 className="text-xl font-semibold mb-2">{job.title}</h2>
                            <p className="mb-1"><span className="font-medium">Status:</span> {JobStatusLabels[job.status] ?? job.status}</p>
                        </div>
                        <button
                            className="mt-4 bg-blue-600 text-white px-4 py-2 rounded hover:bg-blue-700 font-semibold transition"
                            onClick={e => { e.stopPropagation(); navigate(`/jobs/${job.id}`); }}
                        >
                            View Job
                        </button>
                    </div>
                ))}
            </div>
            {jobsResponse && jobsResponse.totalCount === 0 && (
                <div className="text-gray-500 text-center mt-8">No jobs found.</div>
            )}
        </div>
    );
}