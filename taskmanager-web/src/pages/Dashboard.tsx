import { useEffect, useRef, useState } from 'react';
import { api } from '../services/ApiService';
import type { DashboardDataDto } from '../models/DashboardDataDto';

export default function Dashboard() {
    const [profileImageUrl, setProfileImageUrl] = useState<string | null>(null);
    const [dashboardData, setDashboardData] = useState<DashboardDataDto | null>(null);
    const fileInputRef = useRef<HTMLInputElement>(null);

    useEffect(() => {
        fetchProfileImage();
        fetchDashboardData();

        return () => {
            if (profileImageUrl) URL.revokeObjectURL(profileImageUrl);
        };
    }, []);

    const fetchProfileImage = async () => {
        try {
            const blob = await api.get('Users/profile-image', { responseType: 'blob' });
            const url = URL.createObjectURL(blob);
            setProfileImageUrl(url);
        } catch (err) {
            setProfileImageUrl(null);
        }
    };

    const fetchDashboardData = async () => {
        try {
            const data = await api.get<DashboardDataDto>('Dashboard');
            setDashboardData(data);
        } catch (err) {
            setDashboardData(null);
        }
    };

    const handleIconClick = () => {
        fileInputRef.current?.click();
    };

    const handleFileChange = async (e: React.ChangeEvent<HTMLInputElement>) => {
        const file = e.target.files?.[0];
        if (!file) return;
        const formData = new FormData();
        formData.append('file', file);

        try {
            await api.post('Users/profile-image', formData);
            fetchProfileImage();
        } catch (err) {
            setProfileImageUrl(null);
        }
    };

    return (
        <div className="p-4 md:p-8">
            <h1 className="text-3xl font-bold mb-8 text-center text-blue-800">Dashboard</h1>
            {/* Profile Image Section */}
            <div className="flex flex-col items-center mb-10">
                <div className="relative inline-block">
                    {profileImageUrl ? (
                        <img
                            src={profileImageUrl}
                            alt="Profile"
                            className="w-40 h-40 object-cover rounded-full border-4 border-white shadow-lg"
                        />
                    ) : (
                        <div className="w-40 h-40 rounded-full bg-gray-200 flex items-center justify-center text-gray-500 text-xl border-4 border-white shadow-lg">
                            No Image
                        </div>
                    )}
                    <button
                        type="button"
                        onClick={handleIconClick}
                        className="absolute bottom-3 right-3 bg-white border border-gray-300 rounded-full p-2 shadow hover:bg-blue-100 transition"
                        title="Change profile image"
                    >
                        <svg width="20" height="20" fill="none" stroke="currentColor" strokeWidth="2" viewBox="0 0 24 24">
                            <path d="M16.5 3.5a2.121 2.121 0 1 1 3 3L7 19l-4 1 1-4 12.5-12.5z" strokeLinecap="round" strokeLinejoin="round" />
                        </svg>
                    </button>
                    <input
                        ref={fileInputRef}
                        type="file"
                        accept="image/*"
                        style={{ display: 'none' }}
                        onChange={handleFileChange}
                    />
                </div>
            </div>

            {/* Cards Section */}
            <div className="grid grid-cols-1 md:grid-cols-3 gap-8 mb-8">
                {/* Jobs Card */}
                <div className="bg-white rounded-xl shadow-lg p-6 hover:shadow-2xl transition">
                    <h2 className="text-lg font-bold mb-4 text-blue-700">Jobs</h2>
                    {dashboardData ? (
                        <div className="flex flex-col gap-2">
                            <div>
                                <span className="font-semibold text-gray-600">Total: </span>
                                <span className="text-blue-600 font-bold">{dashboardData.jobSummary.total}</span>
                            </div>
                            <div>
                                <span className="font-semibold text-gray-600">Active: </span>
                                <span className="text-yellow-600 font-bold">{dashboardData.jobSummary.active}</span>
                            </div>
                            <div>
                                <span className="font-semibold text-gray-600">Completed: </span>
                                <span className="text-green-600 font-bold">{dashboardData.jobSummary.completed}</span>
                            </div>
                        </div>
                    ) : (
                        <div>Loading...</div>
                    )}
                </div>

                {/* Tasks Card */}
                <div className="bg-white rounded-xl shadow-lg p-6 hover:shadow-2xl transition md:col-span-2">
                    <h2 className="text-lg font-bold mb-4 text-blue-700">Tasks</h2>
                    {dashboardData ? (
                        <div className="grid grid-cols-2 md:grid-cols-3 gap-4">
                            <div>
                                <span className="font-semibold text-gray-600">Total: </span>
                                <span className="text-blue-600 font-bold">{dashboardData.taskSummary.total}</span>
                            </div>
                            <div>
                                <span className="font-semibold text-gray-600">Not Started: </span>
                                <span className="text-gray-500 font-bold">{dashboardData.taskSummary.notStarted}</span>
                            </div>
                            <div>
                                <span className="font-semibold text-gray-600">In Progress: </span>
                                <span className="text-yellow-600 font-bold">{dashboardData.taskSummary.inProgress}</span>
                            </div>
                            <div>
                                <span className="font-semibold text-gray-600">Blocked: </span>
                                <span className="text-red-600 font-bold">{dashboardData.taskSummary.blocked}</span>
                            </div>
                            <div>
                                <span className="font-semibold text-gray-600">Completed: </span>
                                <span className="text-green-600 font-bold">{dashboardData.taskSummary.completed}</span>
                            </div>
                        </div>
                    ) : (
                        <div>Loading...</div>
                    )}
                </div>
            </div>

            {/* Progress Bar */}
            <div className="bg-white rounded-xl shadow-lg p-6 max-w-2xl mx-auto">
                <h2 className="text-lg font-bold mb-4 text-blue-700">Completion</h2>
                {dashboardData ? (
                    <div>
                        <div className="w-full bg-gray-200 rounded-full h-6 mb-2 overflow-hidden">
                            <div
                                className="bg-blue-600 h-6 rounded-full transition-all duration-500"
                                style={{ width: `${dashboardData.progress.overallPercent}%` }}
                            ></div>
                        </div>
                        <div className="text-right font-semibold text-blue-700">
                            {dashboardData.progress.overallPercent}%
                        </div>
                    </div>
                ) : (
                    <div>Loading...</div>
                )}
            </div>
        </div>
    );
}
