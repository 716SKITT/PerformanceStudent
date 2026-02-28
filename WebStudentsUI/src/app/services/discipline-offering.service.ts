import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { Discipline, Semester, StudentGroup, Professor } from './structure.service';

export interface AcademicYearShort {
  id: string;
  startYear: number;
  endYear: number;
}

export interface SemesterWithYear extends Semester {
  academicYear?: AcademicYearShort;
}

export interface DisciplineOffering {
  id: string;
  disciplineId: string;
  semesterId: string;
  studentGroupId: string;
  proffessorId: string;
  discipline?: Discipline;
  semester?: SemesterWithYear;
  studentGroup?: StudentGroup;
  proffessor?: Professor;
}

@Injectable({ providedIn: 'root' })
export class DisciplineOfferingService {
  private apiUrl = `${environment.apiUrl}/DisciplineOffering`;

  constructor(private http: HttpClient) {}

  getAll(): Observable<DisciplineOffering[]> {
    return this.http.get<DisciplineOffering[]>(this.apiUrl);
  }

  getMy(): Observable<DisciplineOffering[]> {
    return this.http.get<DisciplineOffering[]>(`${this.apiUrl}/my`);
  }

  create(payload: {
    disciplineId: string;
    semesterId: string;
    studentGroupId: string;
    proffessorId: string;
  }): Observable<DisciplineOffering> {
    return this.http.post<DisciplineOffering>(this.apiUrl, payload);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
