import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AttendanceService, Attendance } from '../../services/attendance.service';
import { StudentService, Student } from '../../services/student.service';
import { FormsModule } from '@angular/forms';

@Component({
  standalone: true,
  selector: 'app-attendance',
  templateUrl: './attendance.component.html',
  styleUrls: ['./attendance.component.css'],
  imports: [CommonModule, FormsModule]
})
export class AttendanceComponent implements OnInit {
  students: Student[] = [];
  selectedStudentId: string = '';
  attendance: Attendance[] = [];

  constructor(
    private studentService: StudentService,
    private attendanceService: AttendanceService
  ) {}

  ngOnInit(): void {
    this.studentService.getAll().subscribe(s => this.students = s);
  }

  loadAttendance() {
    if (!this.selectedStudentId) return;
    this.attendanceService.getByStudentId(this.selectedStudentId).subscribe(data => {
      this.attendance = data;
    });
  }
}
