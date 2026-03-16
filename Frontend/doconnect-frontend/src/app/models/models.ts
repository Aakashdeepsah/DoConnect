export interface AuthResponse { token: string; username: string; role: string; userId: number; }
export interface LoginRequest  { email: string; password: string; }
export interface RegisterRequest { username: string; email: string; password: string; role: string; }
export interface Question {
  questionId: number; userId: number; username: string; topic: string;
  title: string; questionText: string; status: string;
  imageUrl: string | null; createdAt: string; answerCount: number;
}
export interface Answer {
  answerId: number; questionId: number; userId: number; username: string;
  answerText: string; status: string; imageUrl: string | null; createdAt: string;
}
export interface PendingCount { pendingCount: number; }
