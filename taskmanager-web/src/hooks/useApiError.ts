import { useState } from 'react';
import type { ProblemDetails } from '../models/ProblemDetails';

function isProblemDetails(obj: any): obj is ProblemDetails {
  return obj && typeof obj === 'object' && 'title' in obj && 'status' in obj && 'instance' in obj;
}

export function useApiError<T = any>() {
  const [errorTitle, setErrorTitle] = useState<string | null>(null);
  const [validationErrors, setValidationErrors] = useState<Record<string, string[]> | undefined>();

  function handleApiResponse(result: T | ProblemDetails) {
    if (isProblemDetails(result)) {
      setErrorTitle(result.title);
      setValidationErrors(result.errors);
      return true;
    }
    setErrorTitle(null);
    setValidationErrors(undefined);
    return false;
  }

  return { errorTitle, validationErrors, handleApiResponse };
}