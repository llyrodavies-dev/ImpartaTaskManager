import { BrowserRouter } from 'react-router-dom';
import './App.css';
import AppRoutes from './AppRoutes';
import Navbar from './components/Navbar';

function App() {
  return (
    <BrowserRouter>
      <Navbar />
      <div style={{backgroundColor: 'var(--color-grey-blue-2)' , minHeight: 'calc(100vh - var(--navbar-height))'}}>
        <AppRoutes />
      </div>
    </BrowserRouter>
  );
}

export default App