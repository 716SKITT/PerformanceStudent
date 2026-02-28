import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { GradeService, Grade } from '../../services/grade.service';
import { AssignmentService, Assignment } from '../../services/assignment.service';
import { AuthService, UserAccount } from '../../services/auth.service';
import { StudentService, Student } from '../../services/student.service';
import { DisciplineOfferingService, DisciplineOffering } from '../../services/discipline-offering.service';

@Component({
  selector: 'app-grades',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './grades.component.html',
  styleUrls: ['./grades.component.css']
})
export class GradesComponent implements OnInit {
  role: string | null = null;
  user: UserAccount | null = null;

  grades: Grade[] = [];
  assignments: Assignment[] = [];
  students: Student[] = [];
  offerings: DisciplineOffering[] = [];

  selectedOfferingId = '';
  studentId = '';
  assignmentId: number | null = null;
  score: number | null = null;

  error = '';

  constructor(
    private gradeService: GradeService,
    private assignmentService: AssignmentService,
    private authService: AuthService,
    private studentService: StudentService,
    private offeringService: DisciplineOfferingService
  ) {}

  ngOnInit(): void {
    this.user = this.authService.getUser();
    this.role = this.user?.role ?? null;

    const request$ = this.isProfessor ? this.offeringService.getMy() : this.offeringService.getAll();

    request$.subscribe({
      next: (items) => {
        this.offerings = items;
        if (items.length > 0) {
          this.selectedOfferingId = items[0].id;
        }
        this.loadData();
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

  loadData(): void {
    this.error = '';

    if (this.isProfessor && this.selectedOfferingId) {
      this.assignmentService.getAll(this.selectedOfferingId).subscribe({
        next: (a) => (this.assignments = a),
        error: (e) => (this.error = e?.error?.message ?? 'Не удалось загрузить задания')
      });

      this.gradeService.getByOffering(this.selectedOfferingId).subscribe({
        next: (g) => (this.grades = g),
        error: (e) => (this.error = e?.error?.message ?? 'Не удалось загрузить оценки')
      });

      this.studentService.getAll().subscribe({
        next: (students) => {
          const offering = this.offerings.find((o) => o.id === this.selectedOfferingId);
          this.students = students.filter((s) => s.studentGroupId === offering?.studentGroupId);
        },
        error: (e) => (this.error = e?.error?.message ?? 'Не удалось загрузить студентов')
      });
      return;
    }

    if (this.isStudent && this.user?.linkedPersonId) {
      this.assignmentService.getAll(this.selectedOfferingId || undefined).subscribe({
        next: (a) => (this.assignments = a),
        error: () => {}
      });

      this.gradeService.getByStudent(this.user.linkedPersonId).subscribe({
        next: (g) => {
          if (!this.selectedOfferingId) {
            this.grades = g;
            return;
          }
          this.grades = g.filter((x) => x.assignment?.disciplineOfferingId === this.selectedOfferingId);
        },
        error: (e) => (this.error = e?.error?.message ?? 'Не удалось загрузить оценки')
      });
    }
  }

  addGrade(): void {
    const parsedAssignmentId = Number(this.assignmentId);
    if (!this.studentId || isNaN(parsedAssignmentId) || this.score == null || !this.selectedOfferingId) {
      this.error = 'Заполните студента, задание, оценку и назначение дисциплины';
      return;
    }

    this.gradeService
      .add({
        studentId: this.studentId,
        assignmentId: parsedAssignmentId,
        score: this.score,
        disciplineOfferingId: this.selectedOfferingId
      })
      .subscribe({
        next: () => {
          this.resetForm();
          this.loadData();
        },
        error: (e) => {
          this.error = e?.error?.message ?? 'Не удалось добавить оценку';
        }
      });
  }

  resetForm(): void {
    this.studentId = '';
    this.assignmentId = null;
    this.score = null;
  }

  getAssignmentTitle(id: number): string {
    return this.assignments.find((a) => a.id === id)?.title ?? `Задание #${id}`;
  }

  getStudentName(id: string): string {
    const student = this.students.find((s) => s.id === id);
    return student ? `${student.lastName} ${student.firstName}` : id;
  }

  offeringLabel(offering: DisciplineOffering): string {
    const discipline = offering.discipline?.name ?? 'Дисциплина';
    const group = offering.studentGroup?.name ?? 'Группа';
    const semester = offering.semester?.number ?? '?';
    return `${discipline} · ${group} · семестр ${semester}`;
  }
}
