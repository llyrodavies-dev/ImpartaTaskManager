import { Navigate } from 'react-router-dom';
import { useAuthStorage } from '../hooks/useAuthStorage';
import React from 'react';

interface ProtectedRouteProps {
  children: React.ReactNode;
}

export default function ProtectedRoute({ children }: ProtectedRouteProps) {
  const { getAuth } = useAuthStorage();
  const auth = getAuth();
  // Check for token existence (customize as needed)
  if (!auth.token) {
    return <Navigate to="/signIn" replace />;
  }
  return <>{children}</>;
}
