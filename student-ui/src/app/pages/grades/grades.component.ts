import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { GradeService, Grade } from '../../services/grade.service';
import { AssignmentService, Assignment } from '../../services/assignment.service';
import { AuthService, UserAccount } from '../../services/auth.service';
import { StudentService, Student } from '../../services/student.service';

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

  // Для формы добавления
  studentId: string = '';
  assignmentId: number | null = null;
  score: number | null = null;

  constructor(
    private gradeService: GradeService,
    private assignmentService: AssignmentService,
    private authService: AuthService,
    private studentService: StudentService
  ) {}

  ngOnInit(): void {
    this.user = this.authService.getUser();
    this.role = this.user?.role ?? null;

    if (this.isProfessor) {
      this.loadAssignments();
      this.loadStudentsAndGrades();
    } else if (this.isStudent && this.user?.linkedPersonId) {
      this.loadAssignments();
      this.loadStudentGrades(this.user.linkedPersonId);
    }
  }

  get isProfessor(): boolean {
    return this.role === 'Professor';
  }

  get isStudent(): boolean {
    return this.role === 'Student';
  }

  loadAssignments() {
    this.assignmentService.getAll().subscribe(a => this.assignments = a);
  }

  loadStudentGrades(studentId: string) {
    this.gradeService.getByStudent(studentId).subscribe(g => this.grades = g);
  }

  loadStudentsAndGrades() {
    this.studentService.getAll().subscribe(students => {
      this.students = students;

      const allGrades: Grade[] = [];

      students.forEach(s => {
        if (!s.id) return;
        this.gradeService.getByStudent(s.id).subscribe(grades => {
          allGrades.push(...grades);
          this.grades = [...allGrades]; // триггер перерисовки
        });
      });
    });
  }

  addGrade() {
    const parsedAssignmentId = Number(this.assignmentId);
    if (!this.studentId || isNaN(parsedAssignmentId) || this.score == null) return;

    this.gradeService.add({
      studentId: this.studentId,
      assignmentId: parsedAssignmentId,
      score: this.score
    }).subscribe(() => {
      this.resetForm();
      this.loadStudentsAndGrades();
    });
  }

  resetForm() {
    this.studentId = '';
    this.assignmentId = null;
    this.score = null;
  }

  getAssignmentTitle(id: number): string {
    return this.assignments.find(a => a.id === id)?.title ?? `Задание #${id}`;
  }

  getStudentName(id: string): string {
    const student = this.students.find(s => s.id === id);
    return student ? `${student.lastName} ${student.firstName}` : id;
  }
}
