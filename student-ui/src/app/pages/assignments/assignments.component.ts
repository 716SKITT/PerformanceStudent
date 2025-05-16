import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AssignmentService, Assignment } from '../../services/assignment.service';
import { CourseService, Course } from '../../services/course.service';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-assignments',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './assignments.component.html',
  styleUrls: ['./assignments.component.css']
})
export class AssignmentsComponent implements OnInit {
  assignments: Assignment[] = [];
  courses: Course[] = [];

  // форма добавления
  title = '';
  description = '';
  dueDate = '';
  courseId= '';
  role: string | null = null;
  constructor(
    private assignmentService: AssignmentService,
    private courseService: CourseService,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    this.role = this.authService.getRole();
    this.loadAll();
  }

get isProfessor(): boolean {
  return this.role === 'Professor';
}

get isStudent(): boolean {
  return this.role === 'Student';
}

loadAll() {
  this.assignmentService.getAll().subscribe(a => this.assignments = a);
  this.courseService.getAll().subscribe(c => this.courses = c);
}


  addAssignment() {
    const parsedCourseId = Number(this.courseId);

    if (!this.title || !this.dueDate || isNaN(parsedCourseId) || parsedCourseId <= 0) {
      return;
    }

    this.assignmentService.create({
      title: this.title,
      description: this.description,
      dueDate: new Date(this.dueDate).toISOString(),
      courseId: parsedCourseId
    }).subscribe(() => {
      this.resetForm();
      this.loadAll();
    });
  }


  resetForm() {
    this.title = '';
    this.description = '';
    this.dueDate = '';
    this.courseId = '';
  }

  deleteAssignment(id?: number) {
    if (id == null) return;
    this.assignmentService.delete(id).subscribe(() => this.loadAll());
  }


  getCourseName(courseId?: number): string {
    return this.courses.find(c => c.id === courseId)?.name ?? `Курс #${courseId ?? '?'}`;
  }

}
