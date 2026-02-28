import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DisciplineOfferingService, DisciplineOffering } from '../../services/discipline-offering.service';
import { GradeSheetService, GradeSheet } from '../../services/grade-sheet.service';
import { FinalGradeService, FinalGrade } from '../../services/final-grade.service';
import { Student, StudentService } from '../../services/student.service';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-grade-sheets',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './grade-sheets.component.html',
  styleUrls: ['./grade-sheets.component.css']
})
export class GradeSheetsComponent implements OnInit {
  role: string | null = null;

  offerings: DisciplineOffering[] = [];
  sheets: GradeSheet[] = [];
  finals: FinalGrade[] = [];
  students: Student[] = [];

  selectedOfferingId = '';
  selectedSheetId = '';

  manualScore: Record<string, number | null> = {};
  manualMark: Record<string, string> = {};

  error = '';

  constructor(
    private authService: AuthService,
    private offeringService: DisciplineOfferingService,
    private sheetService: GradeSheetService,
    private finalGradeService: FinalGradeService,
    private studentService: StudentService
  ) {}

  ngOnInit(): void {
    this.role = this.authService.getRole();

    if (!this.isProfessor) {
      this.error = 'Страница доступна только преподавателю.';
      return;
    }

    this.loadOfferings();
    this.studentService.getAll().subscribe({
      next: (items) => (this.students = items),
      error: () => (this.error = 'Не удалось загрузить список студентов')
    });
  }

  get isProfessor(): boolean {
    return this.role === 'Professor';
  }

  loadOfferings(): void {
    this.offeringService.getMy().subscribe({
      next: (items) => (this.offerings = items),
      error: (e) => (this.error = e?.error?.message ?? 'Не удалось загрузить назначения дисциплин')
    });
  }

  onOfferingChange(): void {
    this.selectedSheetId = '';
    this.finals = [];

    if (!this.selectedOfferingId) {
      this.sheets = [];
      return;
    }

    this.sheetService.getByOffering(this.selectedOfferingId).subscribe({
      next: (items) => (this.sheets = items),
      error: (e) => (this.error = e?.error?.message ?? 'Не удалось загрузить ведомости')
    });
  }

  createSheet(): void {
    if (!this.selectedOfferingId) return;

    this.sheetService.create(this.selectedOfferingId).subscribe({
      next: () => this.onOfferingChange(),
      error: (e) => (this.error = e?.error?.message ?? 'Не удалось создать ведомость')
    });
  }

  onSheetChange(): void {
    this.finals = [];
    this.manualScore = {};
    this.manualMark = {};

    if (!this.selectedSheetId) return;

    this.finalGradeService.getBySheet(this.selectedSheetId).subscribe({
      next: (items) => {
        this.finals = items;
        items.forEach((f) => {
          this.manualScore[f.studentId] = f.finalScore ?? null;
          this.manualMark[f.studentId] = f.finalMark ?? '';
        });
      },
      error: (e) => (this.error = e?.error?.message ?? 'Не удалось загрузить итоги')
    });
  }

  recalculate(): void {
    if (!this.selectedSheetId) return;

    this.finalGradeService.recalculate(this.selectedSheetId).subscribe({
      next: (items) => {
        this.finals = items;
        items.forEach((f) => {
          this.manualScore[f.studentId] = f.finalScore ?? null;
          this.manualMark[f.studentId] = f.finalMark ?? '';
        });
      },
      error: (e) => (this.error = e?.error?.message ?? 'Не удалось пересчитать')
    });
  }

  closeSheet(): void {
    if (!this.selectedSheetId) return;

    this.sheetService.close(this.selectedSheetId).subscribe({
      next: () => this.onOfferingChange(),
      error: (e) => (this.error = e?.error?.message ?? 'Не удалось закрыть ведомость')
    });
  }

  saveManual(studentId: string): void {
    if (!this.selectedSheetId) return;

    this.finalGradeService
      .setManual(this.selectedSheetId, studentId, {
        finalScore: this.manualScore[studentId] ?? null,
        finalMark: this.manualMark[studentId] || null
      })
      .subscribe({
        next: () => this.onSheetChange(),
        error: (e) => (this.error = e?.error?.message ?? 'Не удалось сохранить итог')
      });
  }

  statusLabel(status: number): string {
    return status === 1 ? 'Закрыта' : 'Открыта';
  }

  selectedSheetStatus(): number | null {
    const sheet = this.sheets.find((s) => s.id === this.selectedSheetId);
    return sheet ? sheet.status : null;
  }

  canEditFinals(): boolean {
    return this.selectedSheetStatus() === 0;
  }

  getStudentName(studentId: string): string {
    const student = this.students.find((s) => s.id === studentId);
    return student ? `${student.lastName} ${student.firstName}` : studentId;
  }

  studentsForSelectedOffering(): Student[] {
    const offering = this.offerings.find((o) => o.id === this.selectedOfferingId);
    const groupId = offering?.studentGroupId;

    if (!groupId) return this.students;

    return this.students.filter((s) => s.studentGroupId === groupId);
  }
}
