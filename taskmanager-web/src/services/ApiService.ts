import { getAuth, setAuth, clearAuth } from '../utils/authStorage';
export type HttpMethod = 'GET' | 'POST' | 'PUT' | 'PATCH' | 'DELETE';

export interface ApiOptions {
  headers?: Record<string, string>;
  body?: any;
  params?: Record<string, string | number>;
  responseType?: 'json' | 'blob';
}

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'https://localhost:7114/api/';

async function apiRequest<T = any>( path: string, method: HttpMethod = 'GET', options: ApiOptions = {} ): Promise<T> {

  let auth = getAuth();
  const tokenExpiry = auth?.tokenExpiry;
  const refreshToken = auth?.refreshToken;
  const refreshTokenExpiry = auth?.refreshTokenExpiry;

  //console.log('API Request - tokenExpiry:', tokenExpiry, 'refreshToken:', refreshToken, 'refreshTokenExpiry:', refreshTokenExpiry);
  // Check if token is expired
  if (tokenExpiry && new Date(tokenExpiry) <= new Date()) {
    // Check if refresh token is still valid
    if (refreshToken && refreshTokenExpiry && new Date(refreshTokenExpiry) > new Date()) {
      // Attempt to refresh token
      try
      {
        const refreshResponse = await fetch(`${API_BASE_URL}auth/refresh`, {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify({ refreshToken }),
        });
        if (refreshResponse.ok) {
          const newAuth = await refreshResponse.json();
          setAuth(newAuth);
          auth = newAuth;
        } else {
          // Refresh failed, clear auth and throw error
          clearAuth();
          throw new Error('Session expired. Please log in again.');
        }
      } catch (error) {
          clearAuth();
          throw new Error('Session expired. Please log in again.');
      }
    } else {
      // No valid refresh token, clear auth and throw error
      clearAuth();
      throw new Error('Session expired. Please log in again.');
    }
  }

  const { headers, body, params, responseType } = options;
  const url = buildUrl(path, params);
  // Set Content-Type only for JSON requests
  // Needed to make additional changes here for blob/form-data support
  const isBlob = responseType === 'blob';
  const isFormData = body instanceof FormData;

  const fetchHeaders = {
    ...(isBlob ? {} : { 'Content-Type': 'application/json' }),
    ...(auth && auth.token ? { Authorization: `Bearer ${auth.token}` } : {}),
    ...headers,
  };

  // Remove Content-Type for FormData so browser sets it with boundary
  if (isFormData && fetchHeaders['Content-Type']) {
    delete fetchHeaders['Content-Type'];
  }

  try {
    const response = await fetch(url, {
      method,
      headers: fetchHeaders,
      body: isFormData ? body : (body && !isBlob ? JSON.stringify(body) : undefined),
    });
  

    if (isBlob) {
      const blob = await response.blob();
      return blob as T;
    }
    const contentType = response.headers.get('content-type');
    const isJson = contentType && contentType.includes('application/json');
    const data = isJson ? await response.json() : null;
    return data as T;
  }
  catch (error) {
    console.error('Network error:', error);
    throw new Error('Network error. Please check your connection and try again.');
  }
}

function buildUrl(path: string, params?: Record<string, string | number>) {
  const url = new URL(path, API_BASE_URL);
  if (params) {
    Object.entries(params).forEach(([key, value]) => {
      url.searchParams.append(key, String(value));
    });
  }
  return url.toString();
}

export const api = {
  get: <T = any>(path: string, options?: ApiOptions) => apiRequest<T>(path, 'GET', options),
  post: <T = any>(path: string, body?: any, options?: ApiOptions) => apiRequest<T>(path, 'POST', { ...options, body }),
  put: <T = any>(path: string, body?: any, options?: ApiOptions) => apiRequest<T>(path, 'PUT', { ...options, body }),
  patch: <T = any>(path: string, body?: any, options?: ApiOptions) => apiRequest<T>(path, 'PATCH', { ...options, body }),
  delete: <T = any>(path: string, options?: ApiOptions) => apiRequest<T>(path, 'DELETE', options),
};
