// models/models.ts — Sprint 2 additions
export interface AuthResponse {
  token: string;
  username: string;
  role: string;
  userId: number;
  expiresAt?: string; // Sprint 2: ISO date string
}
export interface LoginRequest  { email: string; password: string; }
export interface RegisterRequest {
  username: string; email: string;
  password: string; confirmPassword: string; role: string;
}
export interface Question {
  questionId: number; userId: number; username: string;
  topic: string; title: string; questionText: string;
  status: string; imageUrl: string | null;
  createdAt: string;
  updatedAt?: string | null; // Sprint 2
  answerCount: number;
}
export interface Answer {
  answerId: number; questionId: number; userId: number;
  username: string; answerText: string; status: string;
  imageUrl: string | null; createdAt: string;
  updatedAt?: string | null; // Sprint 2
}
export interface PendingCount { pendingCount: number; }

// Sprint 2: user summary for admin
export interface UserSummary {
  userId: number; username: string; email: string;
  role: string; createdAt: string;
  questionCount: number; answerCount: number;
}
