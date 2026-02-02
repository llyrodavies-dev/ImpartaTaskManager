import type { AuthResult } from '../models/AuthResult';

const AUTH_KEY = 'auth';

export function setAuth(auth: AuthResult) {
  localStorage.setItem(AUTH_KEY, JSON.stringify(auth));
}

export function getAuth(): Partial<AuthResult> {
  const raw = localStorage.getItem(AUTH_KEY);
  if (!raw) return {};
  try {
    return JSON.parse(raw) as AuthResult;
  } catch {
    return {};
  }
}

export function clearAuth() {
  localStorage.removeItem(AUTH_KEY);
}

export { AUTH_KEY };
