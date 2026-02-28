import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { StudentService, StudentSummary } from '../../services/student.service';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-student-finals',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './student-finals.component.html',
  styleUrls: ['./student-finals.component.css']
})
export class StudentFinalsComponent implements OnInit {
  role: string | null = null;
  summary: StudentSummary | null = null;
  error = '';

  constructor(
    private authService: AuthService,
    private studentService: StudentService
  ) {}

  ngOnInit(): void {
    this.role = this.authService.getRole();

    if (!this.isStudent) {
      this.error = 'Страница доступна только студенту.';
      return;
    }

    this.studentService.getMySummary().subscribe({
      next: (data) => (this.summary = data),
      error: (e) => (this.error = e?.error?.message ?? 'Не удалось загрузить итоговые оценки')
    });
  }

  get isStudent(): boolean {
    return this.role === 'Student';
  }
}
