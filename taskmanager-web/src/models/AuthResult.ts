export interface AuthResult {
  token: string;
  email: string;
  userId: string;
  refreshToken: string;
  refreshTokenExpiry: string;
}