import { useState, useEffect } from 'react';
import { api } from '../services/ApiService';
import { useApiError } from '../hooks/useApiError';
import { useAuthStorage } from '../hooks/useAuthStorage';
import type { JobsPagedResponse } from '../models/JobsPagedResponse';


export default function Jobs() {
    const { errorTitle, validationErrors, handleApiResponse } = useApiError();
    const { getAuth } = useAuthStorage();
    const [jobs, setJobs] = useState<JobsPagedResponse | null>(null);

    useEffect(() => {
        const fetchJobs = async () => {
            const auth = getAuth();
            const token = auth.token;
            if (!token) return;
            const result = await api.get('jobs', {
                headers: {
                    Authorization: `Bearer ${token}`,
                },
            });
            if (handleApiResponse(result)) return;
            setJobs(result);
        };
        fetchJobs();
    }, [getAuth, handleApiResponse]);

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
            <ul>
                {jobs?.items.map((job, idx) => (
                    <li key={idx}>{job.title || JSON.stringify(job)}</li>
                ))}
            </ul>
        </div>
    );
}