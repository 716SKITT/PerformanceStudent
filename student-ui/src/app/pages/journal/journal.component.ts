import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { StudentService, Student } from '../../services/student.service';
import { CourseService, Course } from '../../services/course.service';

@Component({
  standalone: true,
  selector: 'app-journal',
  imports: [CommonModule],
  templateUrl: './journal.component.html',
  styleUrls: ['./journal.component.css']
})
export class JournalComponent implements OnInit {
  students: Student[] = [];
  courses: Course[] = [];

  constructor(
    private studentService: StudentService,
    private courseService: CourseService
  ) {}

  ngOnInit(): void {
    this.courseService.getAll().subscribe(c => (this.courses = c));
    this.studentService.getAll().subscribe(s => (this.students = s));
  }

  getCourseName(courseId: number): string {
    return this.courses.find(c => c.id === courseId)?.name ?? `Курс #${courseId}`;
  }
}
