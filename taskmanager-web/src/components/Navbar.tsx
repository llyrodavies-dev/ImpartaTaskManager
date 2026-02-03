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
    <nav style={{ 
      backgroundColor: 'var(--color-navbar)', 
      height: 'var(--navbar-height)', 
      display: 'flex', 
      alignItems: 'center', 
      padding: '0 1rem' ,
      justifyContent: 'space-between',
      paddingLeft:'60px',
      paddingRight:'60px'
      }}>
        <div style={{ display: 'flex', alignItems: 'center' }}>
      <Link to="/" className="text-white mr-4 hover:text-blue-300">Home</Link>
      {isLoggedIn && (
        <>
          <Link to="/dashboard" className="text-white mr-4 hover:text-blue-300">Dashboard</Link>
          <Link to="/jobs" className="text-white mr-4 hover:text-blue-300">My Jobs</Link>
          <Link to="/tasks" className="text-white mr-4 hover:text-blue-300">My Tasks</Link>
        </>
      )}
      </div>
      <div>
        {isLoggedIn ? (
          <button onClick={handleSignOut} className="text-white hover:text-red-400">Sign Out</button>
         ) : (
            <Link to="/signIn" className="text-white hover:text-blue-300">Sign In</Link>
        )}
      </div>
    </nav>
  );
}
