import { Link, useNavigate } from 'react-router-dom';
import { useAuthStorage } from '../hooks/useAuthStorage';

export default function Navbar() {
  const { getAuth, clearAuth } = useAuthStorage();
  const navigate = useNavigate();
  const auth = getAuth();
  const isLoggedIn = !!auth.token;

  const handleSignOut = () => {
    clearAuth();
    navigate('/signIn');
  };

  return (
    <nav className="bg-gray-800 p-4">
      <Link to="/" className="text-white mr-4 hover:text-blue-300">Home</Link>
      <Link to="/about" className="text-white mr-4 hover:text-blue-300">About</Link>
      {isLoggedIn ? (
        <>
          <Link to="/dashboard" className="text-white mr-4 hover:text-blue-300">Dashboard</Link>
          <button onClick={handleSignOut} className="text-white hover:text-red-400">Sign Out</button>
        </>
      ) : (
        <Link to="/signIn" className="text-white hover:text-blue-300">Sign In</Link>
      )}
    </nav>
  );
}
