import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Question } from '../models/models';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class QuestionService {
  private api = `${environment.apiUrl}/question`;
  constructor(private http: HttpClient) {}
  getApproved(): Observable<Question[]>          { return this.http.get<Question[]>(this.api); }
  search(q: string): Observable<Question[]>       { return this.http.get<Question[]>(`${this.api}/search?query=${encodeURIComponent(q)}`); }
  getById(id: number): Observable<Question>       { return this.http.get<Question>(`${this.api}/${id}`); }
  getMine(): Observable<Question[]>               { return this.http.get<Question[]>(`${this.api}/my-questions`); }
  create(fd: FormData): Observable<Question>      { return this.http.post<Question>(this.api, fd); }
}
