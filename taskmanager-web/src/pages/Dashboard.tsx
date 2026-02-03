import { useEffect, useRef, useState } from 'react';
import { api } from '../services/ApiService';
import type { DashboardDataDto } from '../models/DashboardDataDto';
import type { UserInfoDto } from '../models/UserInfoDto';
import type { TasksPagedResponse } from '../models/TasksPagedResponse';
import type { TasksQuery } from '../models/TasksQuery';
import { useApiError } from '../hooks/useApiError';

export default function Dashboard() {
    const { handleApiResponse } = useApiError();
    const [profileImageUrl, setProfileImageUrl] = useState<string | null>(null);
    const [dashboardData, setDashboardData] = useState<DashboardDataDto | null>(null);
    const [userInfo, setUserInfo] = useState<UserInfoDto | null>(null);
    const [tasksResponse, setTasksResponse] = useState<TasksPagedResponse | null>(null);
    const [query] = useState<TasksQuery>({ page: 1, pageSize: 1, sortColumn: 'createdAt', isDescending: false, filters: [{ FieldName: 'Status', Operator: 1, Values: ['4'] }] });
    const fileInputRef = useRef<HTMLInputElement>(null);

    useEffect(() => {
        fetchProfileImage();
        fetchDashboardData();
        fetchUserInfo();
        fetchTasks();

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

    const fetchUserInfo = async () => {
        try {
            const info = await api.get<UserInfoDto>('Users/info');
            setUserInfo(info);
        } catch (err) {
            setUserInfo(null);
        }
    };

    const fetchTasks = async () => {
            const result = await api.post<TasksPagedResponse>('tasks/search', query);
            if (handleApiResponse(result)) return;
            setTasksResponse(result);
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
            <div className="flex flex-col md:flex-row gap-8">
                {/* Profile & Jobs/Tasks Summary */}
                <div className="md:w-1/3 flex flex-col items-center md:items-start">
                    {/* Combined Jobs & Tasks Card */}
                    <div className="w-full bg-white rounded-xl shadow-lg p-6 mb-4 flex flex-col gap-6">
                        {/* Row 1: Profile Image & User Info */}
                        <div className="flex items-center gap-6 mb-2">
                            {/* Profile Image with button */}
                            <div className="relative">
                                {profileImageUrl ? (
                                    <img
                                        src={profileImageUrl}
                                        alt="Profile"
                                        className="w-50 h-50 object-cover rounded-3xl border-4 shadow-lg"
                                        style={{borderColor: "var(--color-navbar)"}}
                                    />
                                ) : (
                                    <div className="w-50 h-50 rounded-3xl bg-gray-200 flex items-center justify-center text-gray-500 text-xl border-4 shadow-lg"  style={{borderColor: "var(--color-navbar)"}}>
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
                                        <path d="M16.5 3.5a2.121 2.122 0 1 1 3 3L7 19l-4 1 1-4 12.5-12.5z" strokeLinecap="round" strokeLinejoin="round" />
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
                            {/* User Info */}
                            {userInfo && (
                                <div>
                                    <div className="text-lg font-semibold text-blue-900 text-left">Name : {userInfo.displayName}</div>
                                    <div className="text-sm text-gray-600 text-left">Email : {userInfo.email}</div>
                                </div>
                            )}
                        </div>
                        {/* Row 2: Jobs Summary */}
                        <div>
                            <h2 className="text-lg font-bold mb-2 text-blue-700 text-left">
                                Jobs: {dashboardData?.jobSummary.total ?? 0}
                            </h2>
                            {dashboardData ? (
                                <div className="flex gap-4 mt-2 mb-4">
                                    <div className="flex flex-col items-center">
                                        <span className="bg-yellow-100 text-yellow-700 font-bold w-14 h-14 flex items-center justify-center rounded-2xl text-2xl mb-1">
                                            {dashboardData.jobSummary.active}
                                        </span>
                                        <span className="text-xs text-gray-600">Active</span>
                                    </div>
                                    <div className="flex flex-col items-center">
                                        <span className="bg-green-100 text-green-700 font-bold w-14 h-14 flex items-center justify-center rounded-2xl text-2xl mb-1">
                                            {dashboardData.jobSummary.completed}
                                        </span>
                                        <span className="text-xs text-gray-600">Completed</span>
                                    </div>
                                    {/* Add more statuses if needed */}
                                </div>
                            ) : (
                                <div>Loading...</div>
                            )}
                        </div>
                        {/* Row 3: Tasks Summary */}
                        <div>
                            <h2 className="text-lg font-bold mb-2 text-blue-700 text-left mt-2">
                                Tasks: {dashboardData?.taskSummary.total ?? 0}
                            </h2>
                            {dashboardData ? (
                                <div className="flex gap-4 mt-2 mb-4">
                                    <div className="flex flex-col items-center">
                                        <span className="bg-gray-200 text-gray-700 font-bold w-14 h-14 flex items-center justify-center rounded-2xl text-2xl mb-1">
                                            {dashboardData.taskSummary.notStarted}
                                        </span>
                                        <span className="text-xs text-gray-600">Not Started</span>
                                    </div>
                                    <div className="flex flex-col items-center">
                                        <span className="bg-yellow-100 text-yellow-700 font-bold w-14 h-14 flex items-center justify-center rounded-2xl text-2xl mb-1">
                                            {dashboardData.taskSummary.inProgress}
                                        </span>
                                        <span className="text-xs text-gray-600">In Progress</span>
                                    </div>
                                    <div className="flex flex-col items-center">
                                        <span className="bg-red-100 text-red-700 font-bold w-14 h-14 flex items-center justify-center rounded-2xl text-2xl mb-1">
                                            {dashboardData.taskSummary.blocked}
                                        </span>
                                        <span className="text-xs text-gray-600">Blocked</span>
                                    </div>
                                    <div className="flex flex-col items-center">
                                        <span className="bg-green-100 text-green-700 font-bold w-14 h-14 flex items-center justify-center rounded-2xl text-2xl mb-1">
                                            {dashboardData.taskSummary.completed}
                                        </span>
                                        <span className="text-xs text-gray-600">Completed</span>
                                    </div>
                                </div>
                            ) : (
                                <div>Loading...</div>
                            )}
                        </div>
                    </div>
                </div>

                {/* Tasks Summary */}
                <div className="md:w-2/3">
                    {/* Progress Bar */}
                    <div className="bg-white rounded-xl shadow-lg p-6">
                        {dashboardData ? (
                            <div>
                                <div className="flex items-center justify-between mb-2">
                                    <span className="text-md font-semibold text-blue-700">Tasks Completed</span>
                                    {/* <span className="text-md font-semibold text-blue-700">
                                        {dashboardData?.progress.overallPercent ?? 0}%
                                    </span> */}
                                </div>
                                <div className="w-full bg-gray-200 rounded-full h-8 shadow-inner relative overflow-hidden">
                                    <div
                                        className="bg-blue-600 h-8 rounded-full flex items-center justify-center transition-all duration-500"
                                        style={{
                                            width: `${dashboardData?.progress.overallPercent ?? 0}%`,
                                            minWidth: dashboardData?.progress.overallPercent ? '2rem' : undefined,
                                        }}
                                    >
                                        <span className="text-white font-bold text-sm drop-shadow">
                                            {dashboardData?.progress.overallPercent ?? 0}%
                                        </span>
                                    </div>
                                </div>
                            </div>
                        ) : (
                            <div>Loading...</div>
                        )}
                    </div>



                    <div className="bg-white rounded-xl shadow-lg p-6 mt-6">
                        <h3 className="text-md font-bold text-red-700 mb-4 text-left">Blocked Tasks</h3>
                        {tasksResponse && tasksResponse.items.filter(task => task.status === 4).length > 0 ? (
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
                                            <th className="px-4 py-2 font-semibold border-b text-blue-800" style={{ borderColor: 'var(--color-grey-blue-1)', borderTopLeftRadius: '0.75rem', width: '120px' }}>Status</th>
                                            <th className="px-4 py-2 font-semibold border-b text-blue-800" style={{ borderColor: 'var(--color-grey-blue-1)', width: '180px' }}>Title</th>
                                            <th className="px-4 py-2 font-semibold border-b text-blue-800" style={{ borderColor: 'var(--color-grey-blue-1)', maxWidth: '500px', width: '500px' }}>Description</th>
                                            <th className="px-4 py-2 font-semibold border-b text-blue-800" style={{ borderColor: 'var(--color-grey-blue-1)', width: '180px' }}>Created At</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        {tasksResponse.items
                                            .filter(task => task.status === 4)
                                            .map((task, idx, arr) => {
                                                const isLastRow = idx === arr.length - 1;
                                                return (
                                                    <tr key={task.id} className="hover:bg-gray-50">
                                                        <td
                                                            className="px-4 py-2 border-b"
                                                            style={{
                                                                borderColor: 'var(--color-grey-blue-1)',
                                                                ...(isLastRow && { borderBottomLeftRadius: '0.75rem' }),
                                                                width: '120px',
                                                            }}
                                                        >
                                                            Blocked
                                                        </td>
                                                        <td className="px-4 py-2 border-b" style={{ borderColor: 'var(--color-grey-blue-1)', width: '180px' }}>
                                                            {task.title}
                                                        </td>
                                                        <td
                                                            className="px-4 py-2 border-b"
                                                            style={{
                                                                borderColor: 'var(--color-grey-blue-1)',
                                                                maxWidth: '500px',
                                                                width: '500px',
                                                                overflow: 'hidden',
                                                                textOverflow: 'ellipsis',
                                                                whiteSpace: 'nowrap'
                                                            }}
                                                            title={task.description}
                                                        >
                                                            {task.description}
                                                        </td>
                                                        <td className="px-4 py-2 border-b" style={{ borderColor: 'var(--color-grey-blue-1)', width: '180px' }}>
                                                            {new Date(task.createdAtUtc).toLocaleString('en-GB', {
                                                                day: '2-digit',
                                                                month: '2-digit',
                                                                year: 'numeric',
                                                                hour: 'numeric',
                                                                minute: '2-digit',
                                                                hour12: false
                                                            }).replace(/\//g, '-')}
                                                        </td>
                                                    </tr>
                                                );
                                            })}
                                    </tbody>
                                </table>
                            </div>
                        ) : (
                            <div className="text-gray-500">No blocked tasks.</div>
                        )}
                    </div>
                </div>
            </div>
        </div>
    );
}
