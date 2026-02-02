export interface AuthResult {
  token: string;
  tokenExpiry: string;
  email: string;
  userId: string;
  refreshToken: string;
  refreshTokenExpiry: string;
}