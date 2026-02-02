import { Routes, Route } from 'react-router-dom';
import Home from './pages/Home';
import About from './pages/About';
import SignIn from './pages/SignIn';
import Dashboard from './pages/Dashboard';
import Jobs from './pages/Jobs';
import ProtectedRoute from './components/ProtectedRoute';
import JobDetails from './pages/JobDetails';
import Tasks from './pages/Tasks';

export default function AppRoutes() {
  return (
    <Routes>
      <Route path="/" element={<Home />} />
      <Route path="/about" element={<About />} />
      <Route path="/signIn" element={<SignIn />} />
      <Route element={<ProtectedRoute />}>
        <Route path="/dashboard" element={<Dashboard />} />
        <Route path="/jobs" element={<Jobs />} />
        <Route path="/jobs/:id" element={<JobDetails />} />
        <Route path="/tasks" element={<Tasks />} />
      </Route>
    </Routes>
  );
}
