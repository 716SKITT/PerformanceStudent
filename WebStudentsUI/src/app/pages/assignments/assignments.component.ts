import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AssignmentService, Assignment } from '../../services/assignment.service';
import { AuthService } from '../../services/auth.service';
import { DisciplineOfferingService, DisciplineOffering } from '../../services/discipline-offering.service';

@Component({
  selector: 'app-assignments',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './assignments.component.html',
  styleUrls: ['./assignments.component.css']
})
export class AssignmentsComponent implements OnInit {
  assignments: Assignment[] = [];
  offerings: DisciplineOffering[] = [];

  title = '';
  description = '';
  dueDate = '';
  selectedOfferingId = '';

  role: string | null = null;
  error = '';

  constructor(
    private assignmentService: AssignmentService,
    private offeringService: DisciplineOfferingService,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    this.role = this.authService.getRole();
    this.loadOfferings();
  }

  get isProfessor(): boolean {
    return this.role === 'Professor';
  }

  get isStudent(): boolean {
    return this.role === 'Student';
  }

  loadOfferings(): void {
    const request$ = this.isProfessor ? this.offeringService.getMy() : this.offeringService.getAll();

    request$.subscribe({
      next: (items) => {
        this.offerings = items;
        if (this.offerings.length > 0 && !this.selectedOfferingId) {
          this.selectedOfferingId = this.offerings[0].id;
        }
        this.loadAssignments();
      },
      error: (e) => {
        this.error = e?.error?.message ?? 'Не удалось загрузить назначения дисциплин';
      }
    });
  }

  loadAssignments(): void {
    this.error = '';
    this.assignmentService.getAll(this.selectedOfferingId || undefined).subscribe({
      next: (items) => {
        this.assignments = items;
      },
      error: (e) => {
        this.error = e?.error?.message ?? 'Не удалось загрузить задания';
      }
    });
  }

  addAssignment(): void {
    if (!this.isProfessor) return;

    if (!this.selectedOfferingId || !this.title || !this.dueDate) {
      this.error = 'Выберите назначение дисциплины, заполните название и дату';
      return;
    }

    this.assignmentService
      .create({
        title: this.title,
        description: this.description,
        dueDate: new Date(this.dueDate).toISOString(),
        disciplineOfferingId: this.selectedOfferingId
      })
      .subscribe({
        next: () => {
          this.resetForm();
          this.loadAssignments();
        },
        error: (e) => {
          this.error = e?.error?.message ?? 'Не удалось создать задание';
        }
      });
  }

  resetForm(): void {
    this.title = '';
    this.description = '';
    this.dueDate = '';
  }

  deleteAssignment(id?: number): void {
    if (!this.isProfessor || id == null) return;

    this.assignmentService.delete(id).subscribe({
      next: () => this.loadAssignments(),
      error: (e) => {
        this.error = e?.error?.message ?? 'Не удалось удалить задание';
      }
    });
  }

  offeringLabel(offering: DisciplineOffering): string {
    const discipline = offering.discipline?.name ?? 'Дисциплина';
    const group = offering.studentGroup?.name ?? 'Группа';
    const semester = offering.semester?.number ?? '?';
    return `${discipline} · ${group} · семестр ${semester}`;
  }
}
