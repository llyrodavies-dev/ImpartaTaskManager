import { useCallback } from 'react';
import type { AuthResult } from '../models/AuthResult';

const AUTH_KEY = 'auth';

export function useAuthStorage() {
  // Store the entire AuthResult object in localStorage
  const setAuth = useCallback((auth: AuthResult) => {
    localStorage.setItem(AUTH_KEY, JSON.stringify(auth));
  }, []);

  const getAuth = useCallback((): Partial<AuthResult> => {
    const raw = localStorage.getItem(AUTH_KEY);
    if (!raw) return {};
    try {
      return JSON.parse(raw) as AuthResult;
    } catch {
      return {};
    }
  }, []);

  const clearAuth = useCallback(() => {
    localStorage.removeItem(AUTH_KEY);
  }, []);

  return { setAuth, getAuth, clearAuth };
}
