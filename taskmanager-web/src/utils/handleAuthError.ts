import type { NavigateFunction } from 'react-router-dom';
import { getAuth } from '../utils/authStorage';

export function handleAuthError(error: any, navigate: NavigateFunction) {
    const auth = getAuth();
    if (!auth || !auth.token) {
        navigate('/signin');
        return true;
    }
    return false;
}