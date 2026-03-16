import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Answer } from '../models/models';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class AnswerService {
  private api = `${environment.apiUrl}/answer`;
  constructor(private http: HttpClient) {}
  getAnswers(qId: number): Observable<Answer[]>  { return this.http.get<Answer[]>(`${this.api}/${qId}`); }
  create(fd: FormData): Observable<Answer>        { return this.http.post<Answer>(this.api, fd); }
}
