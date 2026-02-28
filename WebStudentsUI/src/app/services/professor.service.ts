import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface Professor {
  id: string;
  firstName: string;
  lastName: string;
  gender: number;
  dateOfBirth: string;
  enrollmentDate: string;
  courseId?: number | null;
}

@Injectable({ providedIn: 'root' })
export class ProfessorService {
  private apiUrl = `${environment.apiUrl}/Proffessor`;

  constructor(private http: HttpClient) {}

  getById(id: string): Observable<Professor> {
    return this.http.get<Professor>(`${this.apiUrl}/${id}`);
  }
}
