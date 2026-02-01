export type HttpMethod = 'GET' | 'POST' | 'PUT' | 'PATCH' | 'DELETE';

export interface ApiOptions {
  headers?: Record<string, string>;
  body?: any;
  params?: Record<string, string | number>;
}

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'https://localhost:7114/api/';

async function apiRequest<T = any>( path: string, method: HttpMethod = 'GET', options: ApiOptions = {} ): Promise<T> {
  const { headers, body, params } = options;
  const url = buildUrl(path, params);
  const response = await fetch(url, {
    method,
    headers: {
      'Content-Type': 'application/json',
      ...headers,
    },
    body: body ? JSON.stringify(body) : undefined,
  });

  const contentType = response.headers.get('content-type');
  const isJson = contentType && contentType.includes('application/json');
  const data = isJson ? await response.json() : null;
  return data as T;
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
