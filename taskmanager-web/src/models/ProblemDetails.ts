export interface ProblemDetails {
  type: string;
  title: string;
  details: string;
  status: number;
  errors?: Record<string, string[]>;
  instance: string;
}
