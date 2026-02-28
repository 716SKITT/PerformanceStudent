import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AttendanceService, Attendance } from '../../services/attendance.service';
import { StudentService, Student } from '../../services/student.service';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../services/auth.service';
import { DisciplineOfferingService, DisciplineOffering } from '../../services/discipline-offering.service';

@Component({
  standalone: true,
  selector: 'app-attendance',
  templateUrl: './attendance.component.html',
  styleUrls: ['./attendance.component.css'],
  imports: [CommonModule, FormsModule]
})
export class AttendanceComponent implements OnInit {
  role: string | null = null;

  students: Student[] = [];
  offerings: DisciplineOffering[] = [];
  selectedOfferingId = '';
  selectedStudentId = '';
  attendance: Attendance[] = [];

  attendanceDate = '';
  isPresent = true;

  error = '';

  constructor(
    private studentService: StudentService,
    private attendanceService: AttendanceService,
    private authService: AuthService,
    private offeringService: DisciplineOfferingService
  ) {}

  ngOnInit(): void {
    this.role = this.authService.getRole();

    const request$ = this.isProfessor ? this.offeringService.getMy() : this.offeringService.getAll();

    request$.subscribe({
      next: (items) => {
        this.offerings = items;
        if (items.length > 0) {
          this.selectedOfferingId = items[0].id;
        }
        if (this.isProfessor) {
          this.loadStudents();
        }
        this.loadAttendance();
      },
      error: (e) => {
        this.error = e?.error?.message ?? 'Не удалось загрузить назначения дисциплин';
      }
    });
  }

  get isProfessor(): boolean {
    return this.role === 'Professor';
  }

  get isStudent(): boolean {
    return this.role === 'Student';
  }

  loadStudents(): void {
    if (!this.isProfessor) {
      this.students = [];
      return;
    }

    this.studentService.getAll().subscribe({
      next: (students) => {
        const offering = this.offerings.find((o) => o.id === this.selectedOfferingId);
        if (!offering) {
          this.students = students;
          return;
        }

        this.students = students.filter((s) => s.studentGroupId === offering.studentGroupId);
      },
      error: () => {
        this.error = 'Не удалось загрузить студентов';
      }
    });
  }

  loadAttendance(): void {
    this.error = '';

    if (this.isProfessor && this.selectedOfferingId) {
      this.attendanceService.getByOffering(this.selectedOfferingId).subscribe({
        next: (data) => {
          if (this.selectedStudentId) {
            this.attendance = data.filter((x) => x.studentId === this.selectedStudentId);
          } else {
            this.attendance = data;
          }
        },
        error: (e) => {
          this.error = e?.error?.message ?? 'Не удалось загрузить посещаемость';
        }
      });
      return;
    }

    const currentStudentId = this.authService.getUser()?.linkedPersonId;
    if (this.isStudent && currentStudentId) {
      this.attendanceService.getByStudentId(currentStudentId).subscribe({
        next: (data) => {
          if (this.selectedOfferingId) {
            this.attendance = data.filter((x) => x.disciplineOfferingId === this.selectedOfferingId);
          } else {
            this.attendance = data;
          }
        },
        error: (e) => {
          this.error = e?.error?.message ?? 'Не удалось загрузить посещаемость';
        }
      });
    }
  }

  markAttendance(): void {
    if (!this.isProfessor) return;

    if (!this.selectedOfferingId || !this.selectedStudentId || !this.attendanceDate) {
      this.error = 'Выберите назначение дисциплины, студента и дату';
      return;
    }

    this.attendanceService
      .mark({
        disciplineOfferingId: this.selectedOfferingId,
        studentId: this.selectedStudentId,
        date: new Date(this.attendanceDate).toISOString(),
        isPresent: this.isPresent
      })
      .subscribe({
        next: () => this.loadAttendance(),
        error: (e) => {
          this.error = e?.error?.message ?? 'Не удалось отметить посещаемость';
        }
      });
  }

  offeringLabel(offering: DisciplineOffering): string {
    const discipline = offering.discipline?.name ?? 'Дисциплина';
    const group = offering.studentGroup?.name ?? 'Группа';
    const semester = offering.semester?.number ?? '?';
    return `${discipline} · ${group} · семестр ${semester}`;
  }

  getStudentName(studentId: string): string {
    const student = this.students.find((s) => s.id === studentId);
    return student ? `${student.lastName} ${student.firstName}` : studentId;
  }
}
