import { useState } from 'react';
import { api } from '../services/ApiService';
import type { LoginCommand } from '../models/LoginCommand';
import { useApiError } from '../hooks/useApiError';
import { useAuthStorage } from '../hooks/useAuthStorage';
import { useNavigate, Link } from 'react-router-dom';

export default function SignIn() {
    const { errorTitle, validationErrors, handleApiResponse } = useApiError();
    const { setAuth } = useAuthStorage();

    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const navigate = useNavigate();

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();

        const loginCommand: LoginCommand = { email, password };
        const result = await api.post('auth/login', loginCommand);
        if (handleApiResponse(result)) {
          console.error('Login failed:', result);
        return;
        }

        console.log('Login success:', result);
        setAuth(result);
        navigate('/dashboard');
    };

  return (
    <div className="flex flex-col items-center justify-center" style={{ minHeight: 'calc(100vh - var(--navbar-height))'}}>
      <div className="w-full max-w-md bg-white rounded-lg shadow-md p-8">
        <h1 className="text-3xl font-bold text-center mb-2 text-blue-800" >Job Manager</h1>
        <h2 className="text-lg text-gray-700 text-center mb-6">Sign in to your account</h2>
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
        <form onSubmit={handleSubmit} className="space-y-4">
          <div>
            <label htmlFor="email" className="block text-sm font-medium text-gray-700 mb-1">Email Address</label>
            <input
              id="email"
              type="email"
              value={email}
              onChange={e => setEmail(e.target.value)}
              required
              className="w-full px-3 py-2 border border-gray-300 rounded focus:outline-none focus:ring-2 focus:ring-blue-400"
            />
          </div>
          <div>
            <label htmlFor="password" className="block text-sm font-medium text-gray-700 mb-1">Password</label>
            <input
              id="password"
              type="password"
              value={password}
              onChange={e => setPassword(e.target.value)}
              required
              className="w-full px-3 py-2 border border-gray-300 rounded focus:outline-none focus:ring-2 focus:ring-blue-400"
            />
          </div>
          <button type="submit" className="w-full bg-blue-600 text-white py-2 rounded hover:bg-blue-700 font-semibold transition">
            Sign In
          </button>
        </form>
        <div className="mt-6 text-center">
          <span className="text-gray-600">Don't have an account?</span>
          <Link
            to="/signup"
            className="ml-2 text-blue-600 hover:underline font-semibold"
          >
            Sign Up
          </Link>
        </div>
      </div>
    </div>
  );
}