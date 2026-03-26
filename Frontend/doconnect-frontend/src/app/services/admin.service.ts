// services/admin.service.ts — Sprint 2: added getAllUsers()
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Question, Answer, PendingCount, UserSummary } from '../models/models';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class AdminService {
  private apiUrl = `${environment.apiUrl}/admin`;
  constructor(private http: HttpClient) {}

  getAllQuestions(): Observable<Question[]>  { return this.http.get<Question[]>(`${this.apiUrl}/questions`); }
  getAllAnswers(): Observable<Answer[]>       { return this.http.get<Answer[]>(`${this.apiUrl}/answers`); }
  updateQuestionStatus(id: number, s: string): Observable<any> { return this.http.put(`${this.apiUrl}/questions/${id}/status`, { status: s }); }
  updateAnswerStatus(id: number, s: string): Observable<any>   { return this.http.put(`${this.apiUrl}/answers/${id}/status`, { status: s }); }
  deleteQuestion(id: number): Observable<any> { return this.http.delete(`${this.apiUrl}/questions/${id}`); }
  deleteAnswer(id: number): Observable<any>   { return this.http.delete(`${this.apiUrl}/answers/${id}`); }
  getPendingCount(): Observable<PendingCount> { return this.http.get<PendingCount>(`${this.apiUrl}/pending-count`); }

  // Sprint 2: admin can view all registered users
  getAllUsers(): Observable<UserSummary[]> { return this.http.get<UserSummary[]>(`${this.apiUrl}/users`); }
}
