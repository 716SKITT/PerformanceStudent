import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface GradeSheet {
  id: string;
  disciplineOfferingId: string;
  createdAt: string;
  closedAt?: string | null;
  status: number;
}

@Injectable({ providedIn: 'root' })
export class GradeSheetService {
  private apiUrl = `${environment.apiUrl}/GradeSheet`;

  constructor(private http: HttpClient) {}

  getAll(): Observable<GradeSheet[]> {
    return this.http.get<GradeSheet[]>(this.apiUrl);
  }

  getByOffering(offeringId: string): Observable<GradeSheet[]> {
    return this.http.get<GradeSheet[]>(`${this.apiUrl}/offering/${offeringId}`);
  }

  create(offeringId: string): Observable<GradeSheet> {
    return this.http.post<GradeSheet>(`${this.apiUrl}/${offeringId}/create`, {});
  }

  close(sheetId: string): Observable<GradeSheet> {
    return this.http.post<GradeSheet>(`${this.apiUrl}/${sheetId}/close`, {});
  }
}
