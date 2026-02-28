import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface Student {
  id?: string;
  firstName: string;
  lastName: string;
  gender: number;
  dateOfBirth: string;
  enrollmentDate: string;
  courseId?: number | null;
  studentGroupId?: string | null;
  studentGroup?: {
    id: string;
    name: string;
    studyYear: number;
  } | null;
}

export interface StudentFinalSummaryItem {
  gradeSheetId: string;
  disciplineName: string;
  semesterNumber: number;
  academicYearStart: number;
  academicYearEnd: number;
  finalScore?: number | null;
  finalMark?: string | null;
  updatedAt: string;
}

export interface StudentSummary {
  studentId: string;
  fullName: string;
  finals: StudentFinalSummaryItem[];
}

@Injectable({ providedIn: 'root' })
export class StudentService {
  private apiUrl = `${environment.apiUrl}/Student`;

  constructor(private http: HttpClient) {}

  getAll(): Observable<Student[]> {
    return this.http.get<Student[]>(this.apiUrl);
  }

  getById(id: string): Observable<Student> {
    return this.http.get<Student>(`${this.apiUrl}/${id}`);
  }

  create(student: Student): Observable<Student> {
    return this.http.post<Student>(this.apiUrl, student);
  }

  getMySummary(): Observable<StudentSummary> {
    return this.http.get<StudentSummary>(`${this.apiUrl}/me/summary`);
  }
}
