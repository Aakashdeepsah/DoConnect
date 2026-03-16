import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Question, Answer, PendingCount } from '../models/models';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class AdminService {
  private api = `${environment.apiUrl}/admin`;
  constructor(private http: HttpClient) {}
  getAllQuestions(): Observable<Question[]>                              { return this.http.get<Question[]>(`${this.api}/questions`); }
  getAllAnswers(): Observable<Answer[]>                                  { return this.http.get<Answer[]>(`${this.api}/answers`); }
  updateQuestionStatus(id: number, s: string): Observable<any>          { return this.http.put(`${this.api}/questions/${id}/status`, { status: s }); }
  updateAnswerStatus(id: number, s: string): Observable<any>            { return this.http.put(`${this.api}/answers/${id}/status`, { status: s }); }
  deleteQuestion(id: number): Observable<any>                           { return this.http.delete(`${this.api}/questions/${id}`); }
  deleteAnswer(id: number): Observable<any>                             { return this.http.delete(`${this.api}/answers/${id}`); }
  getPendingCount(): Observable<PendingCount>                           { return this.http.get<PendingCount>(`${this.api}/pending-count`); }
}
