import React from 'react';

export default function Home() {
  return (
    <div className="flex flex-col items-center justify-center min-h-screen px-4" 
    style={{ minHeight: 'calc(100vh - var(--navbar-height))'}}>
      {/* Description Card */}
      <div className="bg-white rounded-xl shadow-lg p-8 max-w-2xl w-full">
        {/* Logo and App Name in Card, Centered */}
        <div className="flex flex-col items-center mb-8">
          <img src="/logo.png" alt="Logo" className="w-20 h-20 mb-4" />
          <span className="text-4xl font-bold main-text">Task Manager</span>
        </div>
        {/* Overview */}
        <section className="mb-6">
          <h2 className="text-2xl font-semibold text-blue-800 mb-2">Overview</h2>
          <p className="text-gray-700">
            <b className="main-text">Task Manager</b> is a modern web application designed to help individuals efficiently manage jobs and tasks. With a clean interface and powerful features, you can easily track progress, collaborate, and stay organized.
          </p>
        </section>
        {/* Key Features */}
        <section className="mb-6">
          <h2 className="text-2xl font-semibold text-blue-800 mb-2">Key Features</h2>
          <ul className="list-disc list-inside text-gray-700 space-y-1">
            <li>Create, edit, and delete jobs and tasks with ease</li>
            <li>Visual dashboards for quick insights into progress</li>
            <li>Status tracking for jobs and tasks (e.g., In Progress, Completed, Blocked)</li>
            <li>Filter and search to find what matters most</li>
            <li>Modern, responsive design for any device</li>
            <li>Secure authentication and user management</li>
          </ul>
        </section>
        {/* Getting Started */}
        <section>
          <h2 className="text-2xl font-semibold text-blue-800 mb-2">Getting Started</h2>
          <ol className="list-decimal list-inside text-gray-700 space-y-1">
            <li>Sign up for a new account or sign in if you already have one.</li>
            <li>Create your first job and add tasks to organize your work.</li>
            <li>Use the dashboard to monitor progress and manage your workload.</li>
            <li>Update statuses and collaborate with your team as needed.</li>
          </ol>
        </section>
      </div>
    </div>
  );
}
