import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface Attendance {
  id?: number;
  studentId: string;
  date: string;
  isPresent: boolean;
  disciplineOfferingId?: string | null;
}

@Injectable({ providedIn: 'root' })
export class AttendanceService {
  private apiUrl = `${environment.apiUrl}/Attendance`;

  constructor(private http: HttpClient) {}

  getByStudentId(studentId: string): Observable<Attendance[]> {
    return this.http.get<Attendance[]>(`${this.apiUrl}/student/${studentId}`);
  }

  getByOffering(offeringId: string): Observable<Attendance[]> {
    return this.http.get<Attendance[]>(`${this.apiUrl}/offering/${offeringId}`);
  }

  mark(attendance: Attendance): Observable<Attendance> {
    return this.http.post<Attendance>(this.apiUrl, attendance);
  }
}
