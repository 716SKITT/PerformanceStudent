import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface AcademicYear {
  id: string;
  startYear: number;
  endYear: number;
}

export interface Semester {
  id: string;
  number: number;
  academicYearId: string;
}

export interface StudentGroup {
  id: string;
  name: string;
  studyYear: number;
  academicYearId: string;
}

export interface Discipline {
  id: string;
  name: string;
  hours: number;
  controlType: number;
}

export interface Professor {
  id: string;
  firstName: string;
  lastName: string;
}

@Injectable({ providedIn: 'root' })
export class StructureService {
  private base = environment.apiUrl;

  constructor(private http: HttpClient) {}

  getAcademicYears(): Observable<AcademicYear[]> {
    return this.http.get<AcademicYear[]>(`${this.base}/AcademicYear`);
  }

  createAcademicYear(payload: Omit<AcademicYear, 'id'>): Observable<AcademicYear> {
    return this.http.post<AcademicYear>(`${this.base}/AcademicYear`, payload);
  }

  deleteAcademicYear(id: string): Observable<void> {
    return this.http.delete<void>(`${this.base}/AcademicYear/${id}`);
  }

  getSemesters(): Observable<Semester[]> {
    return this.http.get<Semester[]>(`${this.base}/Semester`);
  }

  createSemester(payload: Omit<Semester, 'id'>): Observable<Semester> {
    return this.http.post<Semester>(`${this.base}/Semester`, payload);
  }

  deleteSemester(id: string): Observable<void> {
    return this.http.delete<void>(`${this.base}/Semester/${id}`);
  }

  getGroups(): Observable<StudentGroup[]> {
    return this.http.get<StudentGroup[]>(`${this.base}/StudentGroup`);
  }

  createGroup(payload: Omit<StudentGroup, 'id'>): Observable<StudentGroup> {
    return this.http.post<StudentGroup>(`${this.base}/StudentGroup`, payload);
  }

  deleteGroup(id: string): Observable<void> {
    return this.http.delete<void>(`${this.base}/StudentGroup/${id}`);
  }

  getDisciplines(): Observable<Discipline[]> {
    return this.http.get<Discipline[]>(`${this.base}/Discipline`);
  }

  createDiscipline(payload: Omit<Discipline, 'id'>): Observable<Discipline> {
    return this.http.post<Discipline>(`${this.base}/Discipline`, payload);
  }

  deleteDiscipline(id: string): Observable<void> {
    return this.http.delete<void>(`${this.base}/Discipline/${id}`);
  }

  getProfessors(): Observable<Professor[]> {
    return this.http.get<Professor[]>(`${this.base}/Proffessor`);
  }
}
