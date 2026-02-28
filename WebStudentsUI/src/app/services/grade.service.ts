import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { Assignment } from './assignment.service';
export interface Grade {
  id?: number;
  studentId: string;
  assignmentId: number;
  score: number;
  disciplineOfferingId?: string;
  assignment?: Assignment;
}

@Injectable({
  providedIn: 'root'
})
export class GradeService {
  private apiUrl = `${environment.apiUrl}/Grade`;

  constructor(private http: HttpClient) {}

  add(grade: Grade): Observable<Grade> {
    return this.http.post<Grade>(this.apiUrl, grade);
  }

  getByStudent(studentId: string): Observable<Grade[]> {
    return this.http.get<Grade[]>(`${this.apiUrl}/student/${studentId}`);
  }

  getAll(): Observable<Grade[]> {
    return this.http.get<Grade[]>(this.apiUrl);
  }

  getByOffering(offeringId: string): Observable<Grade[]> {
    return this.http.get<Grade[]>(`${this.apiUrl}/offering/${offeringId}`);
  }
}
