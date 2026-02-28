import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface FinalGrade {
  id: string;
  gradeSheetId: string;
  studentId: string;
  finalScore?: number | null;
  finalMark?: string | null;
  updatedAt: string;
}

@Injectable({ providedIn: 'root' })
export class FinalGradeService {
  private apiUrl = `${environment.apiUrl}/FinalGrade`;

  constructor(private http: HttpClient) {}

  getBySheet(sheetId: string): Observable<FinalGrade[]> {
    return this.http.get<FinalGrade[]>(`${this.apiUrl}/sheet/${sheetId}`);
  }

  recalculate(sheetId: string): Observable<FinalGrade[]> {
    return this.http.post<FinalGrade[]>(`${this.apiUrl}/${sheetId}/recalculate`, {});
  }

  setManual(sheetId: string, studentId: string, payload: { finalScore?: number | null; finalMark?: string | null }): Observable<FinalGrade> {
    return this.http.put<FinalGrade>(`${this.apiUrl}/${sheetId}/${studentId}`, payload);
  }
}
