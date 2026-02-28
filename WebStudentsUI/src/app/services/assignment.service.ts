import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface Assignment {
  id?: number;
  title: string;
  description?: string;
  dueDate: string;
  courseId?: number | null;
  disciplineOfferingId?: string | null;
}

@Injectable({
  providedIn: 'root'
})
export class AssignmentService {
  private apiUrl = `${environment.apiUrl}/Assignment`;

  constructor(private http: HttpClient) {}

  getAll(offeringId?: string): Observable<Assignment[]> {
    const url = offeringId ? `${this.apiUrl}?offeringId=${offeringId}` : this.apiUrl;
    return this.http.get<Assignment[]>(url);
  }

  create(assignment: Assignment): Observable<Assignment> {
    return this.http.post<Assignment>(this.apiUrl, assignment);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
