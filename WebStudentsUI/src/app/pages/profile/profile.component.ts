import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuthService, UserAccount } from '../../services/auth.service';
import { Student, StudentService, StudentSummary } from '../../services/student.service';
import { Professor, ProfessorService } from '../../services/professor.service';
import { DisciplineOffering, DisciplineOfferingService } from '../../services/discipline-offering.service';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.css']
})
export class ProfileComponent implements OnInit {
  user: UserAccount | null = null;
  student: Student | null = null;
  studentSummary: StudentSummary | null = null;
  professor: Professor | null = null;
  professorOfferings: DisciplineOffering[] = [];
  error = '';

  constructor(
    private authService: AuthService,
    private studentService: StudentService,
    private professorService: ProfessorService,
    private offeringService: DisciplineOfferingService
  ) {}

  ngOnInit(): void {
    this.user = this.authService.getUser();

    if (!this.user) {
      this.error = 'Не удалось определить текущего пользователя.';
      return;
    }

    if (!this.user.linkedPersonId) {
      return;
    }

    if (this.user.role === 'Student') {
      this.loadStudentProfile(this.user.linkedPersonId);
    }

    if (this.user.role === 'Professor') {
      this.loadProfessorProfile(this.user.linkedPersonId);
    }
  }

  private loadStudentProfile(studentId: string): void {
    this.studentService.getById(studentId).subscribe({
      next: (data) => (this.student = data),
      error: (e) => (this.error = e?.error?.message ?? 'Не удалось загрузить профиль студента')
    });

    this.studentService.getMySummary().subscribe({
      next: (data) => (this.studentSummary = data),
      error: () => {}
    });
  }

  private loadProfessorProfile(professorId: string): void {
    this.professorService.getById(professorId).subscribe({
      next: (data) => (this.professor = data),
      error: (e) => (this.error = e?.error?.message ?? 'Не удалось загрузить профиль преподавателя')
    });

    this.offeringService.getMy().subscribe({
      next: (items) => (this.professorOfferings = items),
      error: () => {}
    });
  }

  get isStudent(): boolean {
    return this.user?.role === 'Student';
  }

  get isProfessor(): boolean {
    return this.user?.role === 'Professor';
  }

  get isAdmin(): boolean {
    return this.user?.role === 'Admin';
  }

  genderLabel(value?: number): string {
    if (value === 0) return 'Женский';
    if (value === 1) return 'Мужской';
    return '—';
  }

  roleLabel(value?: string): string {
    if (value === 'Admin') return 'Администратор';
    if (value === 'Professor') return 'Преподаватель';
    if (value === 'Student') return 'Студент';
    return value ?? '—';
  }
}
