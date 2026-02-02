
import { useCallback } from 'react';
import type { AuthResult } from '../models/AuthResult';
import { getAuth as getAuthStorage, setAuth as setAuthStorage, clearAuth as clearAuthStorage } from '../utils/authStorage';

export function useAuthStorage() {
  // Use shared utility functions, wrapped in useCallback for stable references
  const setAuth = useCallback((auth: AuthResult) => setAuthStorage(auth), []);
  const getAuth = useCallback((): Partial<AuthResult> => getAuthStorage(), []);
  const clearAuth = useCallback(() => clearAuthStorage(), []);
  return { setAuth, getAuth, clearAuth };
}
