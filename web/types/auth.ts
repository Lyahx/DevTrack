export type UserResponse = {
  id: number;
  username: string;
  email: string;
  createdAt: string;
};

export type RegisterRequest = {
  username: string;
  email: string;
  password: string;
};

export type LoginRequest = {
  username: string;
  password: string;
};

export type AuthResponse = {
  token: string;
  expiresAt: string;
  user: UserResponse;
};
