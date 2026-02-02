import { Navigate, Outlet } from 'react-router-dom';
import { useAuthStorage } from '../hooks/useAuthStorage';

export default function ProtectedRoute() {
  const { getAuth } = useAuthStorage();
  const auth = getAuth();

  if (!auth.token) {
    return <Navigate to="/signIn" replace />;
  }
  return <Outlet />;
}
