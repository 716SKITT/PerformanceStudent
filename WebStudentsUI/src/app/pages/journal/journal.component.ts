import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { StudentService, Student } from '../../services/student.service';
import { CourseService, Course } from '../../services/course.service';

@Component({
  standalone: true,
  selector: 'app-journal',
  imports: [CommonModule, FormsModule],
  templateUrl: './journal.component.html',
  styleUrls: ['./journal.component.css']
})
export class JournalComponent implements OnInit {
  students: Student[] = [];
  courses: Course[] = [];
  selectedGroupId = '';
  groups: Array<{ id: string; name: string }> = [];

  constructor(
    private studentService: StudentService,
    private courseService: CourseService
  ) {}

  ngOnInit(): void {
    this.courseService.getAll().subscribe(c => (this.courses = c));
    this.studentService.getAll().subscribe(s => {
      this.students = s;
      this.groups = s
        .filter(x => !!x.studentGroupId && !!x.studentGroup?.name)
        .map(x => ({ id: x.studentGroupId!, name: x.studentGroup!.name }))
        .filter((value, index, self) => self.findIndex(item => item.id === value.id) === index)
        .sort((a, b) => a.name.localeCompare(b.name));
    });
  }

  getCourseName(courseId?: number | null): string {
    if (courseId == null) {
      return '—';
    }
    return this.courses.find(c => c.id === courseId)?.name ?? `Курс #${courseId}`;
  }

  getGroupName(student: Student): string {
    return student.studentGroup?.name ?? 'Не назначена';
  }

  visibleStudents(): Student[] {
    if (!this.selectedGroupId) {
      return this.students;
    }
    return this.students.filter(x => x.studentGroupId === this.selectedGroupId);
  }
}
