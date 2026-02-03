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
            <h1 className="text-3xl font-bold mb-8 text-blue-800">Dashboard</h1>
            {/* Profile Image Section */}
            <div className="flex flex-col md:flex-row gap-8">
                {/* Profile & Jobs Summary */}
                <div className="md:w-1/3 flex flex-col items-center md:items-start">
                    <div className="relative mb-6">
                        {profileImageUrl ? (
                            <img
                                src={profileImageUrl}
                                alt="Profile"
                                className="w-32 h-32 object-cover rounded-3xl border-4 border-white shadow-lg"
                            />
                        ) : (
                            <div className="w-32 h-32 rounded-3xl bg-gray-200 flex items-center justify-center text-gray-500 text-xl border-4 border-white shadow-lg">
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
                    <div className="w-full bg-white rounded-xl shadow-lg p-6 mb-4">
                        <h2 className="text-lg font-bold mb-4 text-blue-700">Jobs</h2>
                        {dashboardData ? (
                            <div className="flex gap-4">
                                <div className="flex flex-col items-center">
                                    <span className="bg-blue-100 text-blue-700 font-bold w-14 h-14 flex items-center justify-center rounded-2xl text-2xl mb-1">{dashboardData.jobSummary.total}</span>
                                    <span className="text-xs text-gray-600">Total</span>
                                </div>
                                <div className="flex flex-col items-center">
                                    <span className="bg-yellow-100 text-yellow-700 font-bold w-14 h-14 flex items-center justify-center rounded-2xl text-2xl mb-1">{dashboardData.jobSummary.active}</span>
                                    <span className="text-xs text-gray-600">Active</span>
                                </div>
                                <div className="flex flex-col items-center">
                                    <span className="bg-green-100 text-green-700 font-bold w-14 h-14 flex items-center justify-center rounded-2xl text-2xl mb-1">{dashboardData.jobSummary.completed}</span>
                                    <span className="text-xs text-gray-600">Completed</span>
                                </div>
                            </div>
                        ) : (
                            <div>Loading...</div>
                        )}
                    </div>
                </div>

                {/* Tasks Summary */}
                <div className="md:w-2/3">
                    <div className="bg-white rounded-xl shadow-lg p-6 mb-8">
                        <h2 className="text-lg font-bold mb-4 text-blue-700">Tasks</h2>
                        {dashboardData ? (
                            <div className="grid grid-cols-2 md:grid-cols-3 gap-4">
                                <div className="flex flex-col items-center">
                                    <span className="bg-blue-100 text-blue-700 font-bold w-14 h-14 flex items-center justify-center rounded-2xl text-2xl mb-1">{dashboardData.taskSummary.total}</span>
                                    <span className="text-xs text-gray-600">Total</span>
                                </div>
                                <div className="flex flex-col items-center">
                                    <span className="bg-gray-200 text-gray-700 font-bold w-14 h-14 flex items-center justify-center rounded-2xl text-2xl mb-1">{dashboardData.taskSummary.notStarted}</span>
                                    <span className="text-xs text-gray-600">Not Started</span>
                                </div>
                                <div className="flex flex-col items-center">
                                    <span className="bg-yellow-100 text-yellow-700 font-bold w-14 h-14 flex items-center justify-center rounded-2xl text-2xl mb-1">{dashboardData.taskSummary.inProgress}</span>
                                    <span className="text-xs text-gray-600">In Progress</span>
                                </div>
                                <div className="flex flex-col items-center">
                                    <span className="bg-red-100 text-red-700 font-bold w-14 h-14 flex items-center justify-center rounded-2xl text-2xl mb-1">{dashboardData.taskSummary.blocked}</span>
                                    <span className="text-xs text-gray-600">Blocked</span>
                                </div>
                                <div className="flex flex-col items-center">
                                    <span className="bg-green-100 text-green-700 font-bold w-14 h-14 flex items-center justify-center rounded-2xl text-2xl mb-1">{dashboardData.taskSummary.completed}</span>
                                    <span className="text-xs text-gray-600">Completed</span>
                                </div>
                            </div>
                        ) : (
                            <div>Loading...</div>
                        )}
                    </div>
                    {/* Progress Bar */}
                    <div className="bg-white rounded-xl shadow-lg p-6">
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
            </div>
        </div>
    );
}
